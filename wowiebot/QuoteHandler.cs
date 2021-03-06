﻿using System;
using System.Collections.Generic;
using System.Timers;

namespace wowiebot
{
    class QuoteHandler
    {
        static QuoteHandler instance = new QuoteHandler();
        public static QuoteHandler getInstance()
        {
            return instance;
        }


        private List<string> quotes;

        private bool addingQuote = false;
        private int lastQuote = -1;
        private List<string> quoteAdders = new List<string>();
        private string quoteToAdd;
        private Timer quoteTimer = new Timer(60000);

        private void QuoteTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string reply = Properties.Settings.Default.nonEmbeddableSrResponse;
            reply = reply.Replace("$SENDER", quoteAdders[0]);
            reply = reply.Replace("$BROADCASTER", ChatHandler.getChannel());
            ChatHandler.sendMessage(reply);
            quoteAdders.Clear();
            addingQuote = false;
            quoteTimer.Stop();
        }

        public void addQuote(string quote, string sender, bool senderIsMod)
        {
            if (quote.Trim() == "")
            {
                return;
            }
            if (Properties.Settings.Default.quotes == null)
            {
                Properties.Settings.Default.quotes = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }
            if (quotes == null)
            {
                string[] arrQuotes = new string[Properties.Settings.Default.quotes.Count];
                Properties.Settings.Default.quotes.CopyTo(arrQuotes, 0);
                quotes = new List<string>(arrQuotes);
                quoteTimer.Elapsed += QuoteTimer_Elapsed;
            }
            if (addingQuote)
            {
                ChatHandler.sendMessage("Finish adding the current quote first.");
                return;
            }

            quoteToAdd = quote;

            if (Properties.Settings.Default.quoteVotersNumber == 0)
            {
                Properties.Settings.Default.quotes.Add(quoteToAdd);
                Properties.Settings.Default.Save();
                quotes.Add(quoteToAdd);
                ChatHandler.sendMessage("Quote added.");
                return;
            }

            else
            {
                addingQuote = true;
                quoteTimer.Start();
                ChatHandler.sendMessage((Properties.Settings.Default.quoteVotersNumber - 1).ToString() +
                             " other " + (Properties.Settings.Default.quoteVotersNumber > 2 ? "people need" : "person needs") + " to agree by typing " +
                             Properties.Settings.Default.prefix +
                             ChatHandler.getVoteYesCommand() +
                             " to add the quote! Ends in one minute.");
                quoteAdders.Add(sender);
            }
        }

        public void voteYes(string sender)
        {
            if (!addingQuote)
                return;

            if (!quoteAdders.Contains(sender))
            {
                quoteAdders.Add(sender);
                if (quoteAdders.Count == Properties.Settings.Default.quoteVotersNumber)
                {
                    wowiebot.Properties.Settings.Default.quotes.Add(quoteToAdd);
                    wowiebot.Properties.Settings.Default.Save();
                    quoteAdders.Clear();
                    quotes.Add(quoteToAdd);
                    addingQuote = false;
                    quoteTimer.Stop();
                    ChatHandler.sendMessage("Quote added.");
                }
            }

            else if (quoteAdders[0] == sender)
            {
                ChatHandler.sendMessage("Yeah, you added the quote. I got it.");
            }

            else
            {
                ChatHandler.sendMessage("You already voted, dingus.");
            }

        }

        public void deconstruct()
        {
            addingQuote = false;
            quoteTimer.Stop();
        }


        public Quote getQuote(string sender)
        {
            if (Properties.Settings.Default.quotes == null)
            {
                Properties.Settings.Default.quotes = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }
            if (quotes == null)
            {
                string[] arrQuotes = new string[Properties.Settings.Default.quotes.Count];
                Properties.Settings.Default.quotes.CopyTo(arrQuotes, 0);
                quotes = new List<string>(arrQuotes);
                quoteTimer.Elapsed += QuoteTimer_Elapsed;
            }
            if (quotes.Count == 0)
            {
                string reply = Properties.Settings.Default.emptyQuotesMessage;
                reply = reply.Replace("$SENDER", sender);
                reply = reply.Replace("$BROADCASTER", ChatHandler.getChannel());
                return new Quote(reply, 0);
            }
            int q;
            do
            {
                q = ChatHandler.rnd.Next(quotes.Count);
            } while (q == lastQuote && quotes.Count > 1);
            lastQuote = q;
            return new Quote(quotes[q], q+1);
        }


        public String getQuote(int i)
        {
            if (Properties.Settings.Default.quotes == null)
            {
                Properties.Settings.Default.quotes = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }
            if (quotes == null)
            {
                string[] arrQuotes = new string[Properties.Settings.Default.quotes.Count];
                Properties.Settings.Default.quotes.CopyTo(arrQuotes, 0);
                quotes = new List<string>(arrQuotes);
                quoteTimer.Elapsed += QuoteTimer_Elapsed;
            }
            if (quotes.Count > i)
            {
                return quotes[i];
            }
            else
            {
                return "Index out of range.";
            }
        }
    }
}
