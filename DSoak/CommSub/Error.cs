using System;
using System.Collections.Generic;

namespace CommSub
{
    public class Error
    {
        private static readonly Dictionary<StandardErrorNumbers, Error> StandardErrors;

        public enum StandardErrorNumbers
        {
            NullEnvelopeOrMessage = 1000,
            InvalidTypeOfMessage = 1001,
            UnknownEndPoint = 1002,
            InvalidProcessInfo = 1003,
            ProcessIsAlreadyRegistered = 1004,
            InvalidProcessId = 1005,
            UnknownProcessId = 1006,
            InvalidGameInfo = 1007,
            GameAlreadyRegistered = 1008,
            InvalidGameId = 1009,
            UnknownGameId = 1010,
            InvalidProcessInformation = 1011,
            EndPointCannotBeNull = 1012,
            UnexpectedAliveRequest = 1013,
            NullMessageNumber = 1014,
            NullConversationId = 1015,
            UnkownIdentity = 1016,

            ProcessIdInMessageNumberMustBeZero = 1019,
            ProcessIdInMessageNumberCannotBeZero = 1020,
            ProcessIdInMessageNumberIsNotAProcessId = 1021,
            SendersEndPointDoesNotMatchProcessEndPoint = 1022,

            ProcessNotInGame = 1030,
            NoContext = 1031,
            MessageNotFromGameServer = 1032,

            ProcessCannotBeRemovedFromGame = 1101,

            ProcessIsAlreadyPartOfGame = 1201,
            JoinRequestIsNotForCurrentGame = 1202,
            JoinRequestIsOnlyValidForAvailableGames = 1203,
            JoinRequestIsIncomplete = 1204,
            ProcessCannotBeAddedToGame = 1205,
            PlayerInformationCannotBeNull=1206,
            CannotValidateProcess = 1207,

            InvalidResourceType = 1301,

            StartGameProtocolFailed = 1401,

            AttackingProcessIsNotAPlayer = 1500,
            TargetProcessIsInvalid = 1501,

            InvalidTypeOfProcess = 1701,

            InvalidResource = 2000,

            InvalidConversationSetup = 3000,
            InvalidConversationState = 3001,
            InvalidProcessStateForConversation = 3002

        }

        public StandardErrorNumbers Number { get; set; }
        public string Message { get; set; }

        private DateTime _createdOnTimestamp;
        public DateTime Timestamp { get { return _createdOnTimestamp; } }

        static Error()
        {
            // Registry errors
            StandardErrors = new Dictionary<StandardErrorNumbers, Error>();
            Add(StandardErrorNumbers.NullEnvelopeOrMessage,     "Empty Envelope or Message");
            Add(StandardErrorNumbers.InvalidTypeOfMessage, "Invalid Type of Message");
            Add(StandardErrorNumbers.UnknownEndPoint, "Unknown End Point -- an unregistered process may be trying to communicate"); 
            Add(StandardErrorNumbers.InvalidProcessInfo, "Invalid Process Information.  To register a process, it must have an end point, a label, and be either a game manager or player");
            Add(StandardErrorNumbers.ProcessIsAlreadyRegistered, "Process is available registered -- treating as logged in");
            Add(StandardErrorNumbers.InvalidProcessId, "Invalid Process Id");
            Add(StandardErrorNumbers.UnknownProcessId, "Unknown Process Id");
            Add(StandardErrorNumbers.InvalidGameInfo, "Invalid Game Information");
            Add(StandardErrorNumbers.GameAlreadyRegistered, "Game Already Registered");
            Add(StandardErrorNumbers.InvalidGameId, "Invalid Game Id");
            Add(StandardErrorNumbers.UnknownGameId, "Unknown Game Id");
            Add(StandardErrorNumbers.InvalidProcessInformation, "Invalid Process Information");
            Add(StandardErrorNumbers.EndPointCannotBeNull, "End Point Cannot Be Null");
            Add(StandardErrorNumbers.UnexpectedAliveRequest, "Received an unexpected alive request -- process not registered");
            Add(StandardErrorNumbers.NullMessageNumber, "Invalid message because it contains a null message number");
            Add(StandardErrorNumbers.NullConversationId, "Invalid conversation because it contains a null conversation id");
            Add(StandardErrorNumbers.UnkownIdentity, "Unknown user identity");

            // ??
            Add(StandardErrorNumbers.ProcessIdInMessageNumberMustBeZero, "The Process Id in the message must be zero");
            Add(StandardErrorNumbers.ProcessIdInMessageNumberCannotBeZero, "The Process Id in the message cannot be zero");
            Add(StandardErrorNumbers.ProcessIdInMessageNumberIsNotAProcessId, "The process Id in message number is not an agent Id");
            Add(StandardErrorNumbers.SendersEndPointDoesNotMatchProcessEndPoint, "Senders end point does not match agents end point");
            Add(StandardErrorNumbers.ProcessNotInGame,"The agent is not in the specified game. Check the message content in the logs.");
            Add(StandardErrorNumbers.NoContext, "No context provided to the conversation strategy.  Check logs that is trying to execute the strategy.");
            Add(StandardErrorNumbers.MessageNotFromGameServer, "The message received is not from the Game Server, as expected.  Check logs for EndPoint information.");
            Add(StandardErrorNumbers.ProcessCannotBeRemovedFromGame, "Process cannot be removed from game");

            // Join Game protocol
            Add(StandardErrorNumbers.ProcessIsAlreadyPartOfGame, "Process is already part of this game -- no action needed");
            Add(StandardErrorNumbers.JoinRequestIsNotForCurrentGame, "Join request is not for the current game, i.e., wrong game Id -- try joining another game");
            Add(StandardErrorNumbers.JoinRequestIsOnlyValidForAvailableGames, "Join request is only valid for AVAILABLE games -- try joining another game");
            Add(StandardErrorNumbers.JoinRequestIsIncomplete, "Join request is incomplete -- check request for missing information like A# or names");
            Add(StandardErrorNumbers.ProcessCannotBeAddedToGame, "Process cannot be added to game -- look at GameServer log for more details");
            Add(StandardErrorNumbers.PlayerInformationCannotBeNull, "Process information cannot be null");
            Add(StandardErrorNumbers.CannotValidateProcess, "Cannot validate process");

            // Get-resouce related protocols
            Add(StandardErrorNumbers.InvalidResourceType, "Invalid Resource Type -- be sure the resource type is one the receiver of your request can handle");

            // Start protocol
            Add(StandardErrorNumbers.StartGameProtocolFailed, "Start Game Protocol Failed - game is shutting done");
            Add(StandardErrorNumbers.AttackingProcessIsNotAPlayer, "The attacking agent is not a player.  Only a player can send this kind of message");
            Add(StandardErrorNumbers.TargetProcessIsInvalid, "The target agent is either not in the game or the wrong kind of agent");
            Add(StandardErrorNumbers.InvalidResource, "Invalid Resource.  It may have been used before.");
            Add(StandardErrorNumbers.InvalidTypeOfProcess, "Invalid Type of Process");

            // General protocol errors
            Add(StandardErrorNumbers.InvalidConversationSetup, "Conversation is not setup probably and cannot be started");
            Add(StandardErrorNumbers.InvalidConversationState, "Invalid conversation state -- aborting conversation");
            Add(StandardErrorNumbers.InvalidProcessStateForConversation, "Invalid process state for conversation");
        }

        public static Error Get(StandardErrorNumbers index)
        {
            Error error;
            if (StandardErrors.ContainsKey(index))
                error = StandardErrors[index].MemberwiseClone() as Error;
            else
                error = new Error { Message = string.Format("Unknown Error {0}", index) };

            error._createdOnTimestamp = DateTime.Now;
            return error;
        }

        private static void Add(StandardErrorNumbers number, string message)
        {
            StandardErrors.Add(number, new Error() { Number = number, Message = message });
        }

    }
}

