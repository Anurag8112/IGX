﻿using System;
using QuickFix;
using QuickFix.Fields;
using QuickFix.Transport;

namespace IGX.FIX_Protocol
{
    public class IGXQuickFixApp : MessageCracker, IApplication
    {
        private SessionID _session;

        public void FromAdmin(QuickFix.Message message, SessionID sessionID)
        {
            Console.WriteLine($"Received ADMIN message: {message}");
        }

        public void FromApp(QuickFix.Message message, SessionID sessionID)
        {
            try
            {
                // Crack the message to handle based on message type
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling incoming message: {ex.Message}");
            }
        }

        public void OnCreate(SessionID sessionID)
        {
            Console.WriteLine("Session created - " + sessionID);
        }

        public void OnLogout(SessionID sessionID)
        {
            Console.WriteLine("Logged out - " + sessionID);
        }

        public void OnLogon(SessionID sessionID)
        {
            Console.WriteLine("Logged in - " + sessionID);
        }

        public void ToAdmin(QuickFix.Message message, SessionID sessionID)
        {
            if (message is QuickFix.FIX44.Logon logon)
            {
                // Check if it's a failover scenario
                if (sessionID.TargetCompID.Equals("SecondaryGateway") && !logon.ResetSeqNumFlag.Obj)
                {
                    // Failover scenario - set sequence number to 1 and ResetSeqNumFlag to Y
                    logon.Set(new QuickFix.Fields.EncryptMethod(0)); // Use the correct encryption method value
                    logon.Set(new QuickFix.Fields.ResetSeqNumFlag(true));
                    logon.Header.SetField(new MsgSeqNum(1));
                }
                else
                {
                    // Normal login scenario - do nothing special
                }
            }
        }

        public void ToApp(QuickFix.Message message, SessionID sessionID)
        {
            Console.WriteLine($"Sending message: {message}");
        }

        // Implement the specific messages you expect to receive
        public void OnMessage(QuickFix.FIX44.ExecutionReport message, SessionID sessionID)
        {
            Console.WriteLine($"Received ExecutionReport: {message}");
        }

        public void Connect()
        {
            try
            {
                SessionSettings settings = new SessionSettings("quickfix.cfg");
                IGXQuickFixApp application = new IGXQuickFixApp();
                FileStoreFactory storeFactory = new FileStoreFactory(settings);
                ScreenLogFactory logFactory = new ScreenLogFactory(settings);
                var messageFactory = new DefaultMessageFactory();
                SocketInitiator initiator = new SocketInitiator(application, storeFactory, settings, logFactory, messageFactory);

                // Start the initiator and initiate the connection
                initiator.Start();
                _session = initiator.GetSessionIDs().GetEnumerator().Current;

                // Wait for user input to stop the application
                Console.WriteLine("Press <Enter> to quit.");
                Console.ReadLine();

                // Stop the initiator and close the connection
                initiator.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
