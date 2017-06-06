using System;
using System.Collections.Generic;

namespace cards
{
    class Program
    {
        static void Main(string[] args)
        {
            //instantiate and shuffle deck
            Deck d = new Deck();
            d.shuffle();

            //set up players
            Player g = new Player("Graham");
            Player m = new Player("Mat");
            Player n = new Player("Noelle");
            Player[] players = new Player[]{g,m,n};
            //deal initial hidden castle (3 each)
            for (int i = 0; i < 3; i++) {
                g.dealHiddenCastle(d);
                m.dealHiddenCastle(d);
                n.dealHiddenCastle(d);
            }

            //deal 6 cards each
            for (int i = 0; i < 6; i++) {
                g.draw(d);
                m.draw(d);
                n.draw(d);
            }

            //each player chooses 3 to add to visible castle
            foreach (Player p in players) {
                Console.WriteLine($"{p.name}, here are your cards:");
                for (int i = 0; i < 3; i++) {
                    int counter = 0;
                    foreach(Card c in p.hand) {
                        Console.WriteLine(counter + " - " + c.ToString() + " ");
                        counter++;
                    }
                    Console.WriteLine("Which card would you like to add to your castle? One at a time please!");
                    int response = playSelection(p.hand.Count);
                    p.castle.Add(p.discard(response));
                }
            }
            
            //cards currently in play
            List<Card> inPlay = new List<Card>();

            //begin player turn
            int playCount = 0;
            int turn = playCount % players.Length;
            Console.WriteLine($"{players[turn].name}, your turn! Ready? Press any key(s) to continue! And enter!");
            Console.ReadLine();
            //show current inplay deck (top 3 cards and total card count)
            Console.Write("Top Cards: ");
            for (int i = 0; i < inPlay.Count; i++) {
                if (i == 3) { break; }
                Console.Write(inPlay[inPlay.Count-i-1].ToString());
            }
            Console.Write($"...({inPlay.Count} total cards in the pot)");
            foreach (Card c in players[turn].hand) {
                int counter = 0;
                Console.WriteLine(counter + " - " + c.ToString() + " ");
                counter++;
            }
            //future feature: players can play multiple cards if they have duplicates
            //prompt user for card to play
            Console.WriteLine("Which card would you like to play?");
            int topVal;
            if (inPlay.Count == 0) {
                topVal = 0;
            }
            else {
                topVal = inPlay[inPlay.Count-1].val;
            }
            int r = playSelection(players[turn].hand.Count, topVal, players[turn].hand);
            inPlay.Add(players[turn].discard(r));
        }

        public static int playSelection(int numCards) {
            string response = Console.ReadLine();
            int choice;
            //if invalid choice
            if(!int.TryParse(response, out choice) || choice >= numCards || choice < 0) {
                System.Console.WriteLine("I'm sorry but that wasn't a recognizable choice. Please try again.");
                playSelection(numCards);
            }
            return choice;
        }
        public static int playSelection(int numCards, int topVal, List<Card> playerCards) {
            int choice = playSelection(numCards);
            if (playerCards[choice].val == 10 || playerCards[choice].val == 3 || playerCards[choice].val == 7 ){
                return choice;
            }
            else if (playerCards[choice].val < topVal) {
                System.Console.WriteLine("uh, no.");
                return playSelection(numCards, topVal, playerCards);
            }
            else {
                return choice;
            }
        }
    }
}
