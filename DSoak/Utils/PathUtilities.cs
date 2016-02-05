using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Utils
{
	/// <summary>
	/// Utilities pertaining to paths.
	/// </summary>
    public static class PathUtilities
    {
        /// <summary>
        /// The path and filename of the application.
        /// </summary>
        public static string ExecutablePath
        {
            get
            {
                if (Assembly.GetEntryAssembly() != null)
                    return Assembly.GetEntryAssembly().Location;
                else if (Assembly.GetExecutingAssembly() != null)
                    return Assembly.GetExecutingAssembly().Location;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// The path of the application.
        /// </summary>
        public static string ApplicationPath
        {
            get { return Path.GetDirectoryName(ExecutablePath); }
        }

        /// <summary>
        /// The version of the application.
        /// </summary>
        public static string ApplicationVersion
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                AssemblyInformationalVersionAttribute[] attribs =
                    (AssemblyInformationalVersionAttribute[])
                    assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if ((attribs != null) && (attribs.Length > 0))
                    return attribs[0].InformationalVersion.Trim();

                throw new ApplicationException("Assembly doesn't have informational version.");
            }
        }

        /// <summary>
        /// The name of the application.
        /// </summary>
        public static string ApplicationName
        {
            get { return Path.GetFileNameWithoutExtension(ExecutablePath); }
        }

        /// <summary>
        /// Changes an absolute path into a relative path.  The
        /// returned path is relative to the application path.
        /// </summary>
        /// <param name="filePath">The absolute path.</param>
        /// <returns>A path relative to the application path.</returns>
        public static string RelativeAppPath(string filePath)
        {
            return RelativePath(ApplicationPath, filePath);
        }

        /// <summary>
        /// Changes the filePath to be relative to the basePath.
        /// </summary>
        /// <param name="basePath">The base of the relative path.</param>
        /// <param name="filePath">The absolute path.</param>
        /// <returns>The relative path.</returns>
        public static string RelativePath(string basePath, string filePath)
        {
            if ((filePath == null) ||
                (filePath == "") ||
                (filePath == string.Empty))
                return "";

            string[] baseArr = basePath.Split(Path.DirectorySeparatorChar);
            string[] fileArr = filePath.Split(Path.DirectorySeparatorChar);

            // Check the directory
            if (baseArr[0] != fileArr[0])
                return filePath;

            int fileLength = fileArr.Length;
            int baseLength = baseArr.Length;

            while (fileLength > 0 && fileArr[fileLength - 1].Trim() == "")
                fileLength--;

            while (baseLength > 0 && baseArr[baseLength - 1].Trim() == "")
                baseLength--;

            int length = Math.Min(fileLength, baseLength);

            string relativePath = "";

            int index;
            for (index = 0; index < length; index++)
            {
                // if the two strings are not equal then quit
                if (string.Compare(fileArr[index], baseArr[index], true) != 0)
                    break;
            }

            for (int j = index; j < fileLength; j++)
            {
                if (fileArr[j] != "")
                    relativePath += fileArr[j] + ((j != fileLength - 1) ? Path.DirectorySeparatorChar.ToString() : "");
            }

            for (int k = index; k < baseLength; k++)
                relativePath = ".." + Path.DirectorySeparatorChar + relativePath;

            relativePath = "." + Path.DirectorySeparatorChar + relativePath;

            return relativePath;
        }

        /// <summary>
        /// Changes a relative path into an absolute path.
        /// </summary>
        /// <param name="filePath">A path relative to the application path.</param>
        /// <returns>An absolute path.</returns>
        public static string AbsoluteAppPath(string filePath)
        {
            return AbsolutePath(ApplicationPath, filePath);
        }

        /// <summary>
        /// Changes the relativePath to an absolute path.
        /// </summary>
        /// <param name="basePath">The path that will become the new root for the given relative path.</param>
        /// <param name="relativePath">The path to convert to an absolute, rooted path.</param>
        /// <returns>Returns the absolute, rooted path generated from the basePath and relativePath</returns>
        public static string AbsolutePath(string basePath, string relativePath)
        {
            if ((relativePath == null) ||
                (relativePath == "") ||
                (relativePath == string.Empty))
                return basePath;

            // If the path is already rooted, it is absolute.
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            // Trim off the leading "current directory" token
            if (relativePath.StartsWith(@".\"))
                relativePath = relativePath.Substring(2);

            // Check to see if we're already done
            if (relativePath == "")
                return basePath;

            // Trim off the trailing \ from the base path if it exists
            if (basePath.EndsWith(@"\"))
                basePath = basePath.Substring(0, basePath.Length - 1);

            int startIndex = 0;
            while ((startIndex < relativePath.Length) &&
                   (relativePath.Substring(startIndex, 3) == (".." + Path.DirectorySeparatorChar)))
            {
                // Move the base up one level in response to the "previous directory" token
                DirectoryInfo di = Directory.GetParent(basePath);

                if (di == null)
                    return Path.GetFileName(relativePath);

                basePath = di.FullName;
                startIndex += 3;
            }

            return Path.Combine(basePath, relativePath.Substring(startIndex));
        }


        public static string FindFile(params string[] guesses)
        {
            int guessIndex = 0;
            string filePath = string.Empty;
            while (guessIndex < guesses.Length && filePath == string.Empty)
            {
                if (File.Exists(guesses[guessIndex]))
                    filePath = guesses[guessIndex];
                guessIndex++;
            }
            return filePath;
        }

        # region Thread-Safe Path Utilities

        /// <summary>
        /// Dictionary used so that path formatters can get
        /// the correct filenames when loading files.
        /// </summary>
        private static Dictionary<Thread, List<string>> _threadFilenames = new Dictionary<Thread, List<string>>();

        /// <summary>
        /// Get the current filename associated with this thread.
        /// When a file is loaded, it can push the filename on the stack and
        /// make it available to other code sections that need to utilize the
        /// loading filename.
        /// </summary>
        /// <returns></returns>
        public static string GetFilename()
        {
            Thread thread = Thread.CurrentThread;
            if (!_threadFilenames.ContainsKey(thread))
                return string.Empty;

            List<string> filenames = _threadFilenames[thread];
            return filenames[filenames.Count - 1];
        }

        /// <summary>
        /// Push a filename on the stack, and associated it with this thread.
        /// </summary>
        /// <param name="filename">The filename to push onto the stack.</param>
        public static void PushFilename(string filename)
        {
            Thread thread = Thread.CurrentThread;
            if (!_threadFilenames.ContainsKey(thread))
                _threadFilenames.Add(thread, new List<string>());

            _threadFilenames[thread].Add(filename);
        }

        /// <summary>
        /// Pop the filename off the stack.
        /// </summary>
        /// <param name="filename">The filename to pop off the stack.</param>
        public static void PopFilename(string filename)
        {
            Thread thread = Thread.CurrentThread;
            if (!_threadFilenames.ContainsKey(thread))
                return;

            List<string> filenames = _threadFilenames[thread];
            filenames.Remove(filename);
            if (filenames.Count == 0)
                _threadFilenames.Remove(thread);
        }

        # endregion
    }
}
