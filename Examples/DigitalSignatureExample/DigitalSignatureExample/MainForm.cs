using System;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace DigitalSignatureExample
{
    public partial class MainForm : Form
    {
        private RSACryptoServiceProvider _rsa;
        private RSAParameters _senderPublicKeyInfo;

        private byte[] _encryptedmessageHash;

        public MainForm()
        {
            InitializeComponent();
        }

        private void generateKeysButton_Click(object sender, EventArgs e)
        {
            // This show how to create a public/private key pair.  Typically a process would only do this once.
            // An alternative would be load an exiting key pair from a local key store or file.

            _rsa = new RSACryptoServiceProvider();
            _senderPublicKeyInfo = _rsa.ExportParameters(true);
            publicKey.Text = FormatByteArray(_senderPublicKeyInfo.Exponent) + FormatByteArray(_senderPublicKeyInfo.Modulus);
            privateKey.Text = FormatByteArray(_senderPublicKeyInfo.D) + FormatByteArray(_senderPublicKeyInfo.Modulus);
        }

        private void signButton_Click(object sender, EventArgs e)
        {
            // This shows how to sign a chunk of data (message, document, etc.) using a key pair.

            // Generate a hash of the message.  SHA1Managed is a class the encapsulated the SHA1 hash
            // alogrithm -- an good algorithm for our purposes here.  It produces a 20-byte hash code from
            // any array of bytes.
            SHA1Managed hasher = new SHA1Managed();
            byte[] messageBytes = Encoding.Unicode.GetBytes(sampleMessage.Text);
            byte[] messageHash = hasher.ComputeHash(messageBytes);

            hash.Text = FormatByteArray(messageHash);

            // Create a digital signature using a Signatur Formatter and the hash.
            RSAPKCS1SignatureFormatter rsaSigner = new RSAPKCS1SignatureFormatter(_rsa);
            rsaSigner.SetHashAlgorithm("SHA1");
            _encryptedmessageHash = rsaSigner.CreateSignature(messageHash);

            encryptedHash.Text = FormatByteArray(_encryptedmessageHash);
        }

        private void verifyButton_Click(object sender, EventArgs e)
        {
            // This logic would typically be in the receiving program
            //      Setup RSAParameter using public key data (keyInfo.exponent, keyInfo.modulus)
            //      Create RSACryptoServiceProviders and import keyInfo

            // Import the public key into a Cryptography Service Provider
            RSAParameters receiverRSAKeyInfo = new RSAParameters();
            receiverRSAKeyInfo.Modulus = _senderPublicKeyInfo.Modulus;
            receiverRSAKeyInfo.Exponent = _senderPublicKeyInfo.Exponent;

            RSACryptoServiceProvider receiverRSA = new RSACryptoServiceProvider();
            receiverRSA.ImportParameters(receiverRSAKeyInfo);

            // Generate a hash of transmitted message
            SHA1Managed hasher = new SHA1Managed();
            byte[] messageBytes = Encoding.Unicode.GetBytes(sampleMessage.Text);
            byte[] messageHash = hasher.ComputeHash(messageBytes);

            //  Setup a deformatter for comparing the new hash against the transmitted hash,
            // using the public key
            RSAPKCS1SignatureDeformatter rsaSignComparer = new RSAPKCS1SignatureDeformatter(_rsa);
            rsaSignComparer.SetHashAlgorithm("SHA1");

            //  Do the comparision
            bool verified = rsaSignComparer.VerifySignature(messageHash, _encryptedmessageHash);
            verifyResults.Text = verified ? @"VERIFIED" : @"NOT VERIFIED";
        }

        private string FormatByteArray(byte[] data)
        {
            string result = "";
            foreach (byte d in data)
                result += string.Format("{0} ", d);
            return result;
        }
    }
}
