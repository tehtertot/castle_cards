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
            bool gameIsGood = true;
            int playCount = 0;
            while (gameIsGood) {//begin player turn
                int turn = playCount % players.Length;
                Console.WriteLine($"{players[turn].name}, your turn! Ready? Press any key(s) to continue! And enter!");
                Console.ReadLine();
                //show current inplay deck (top 3 cards and total card count)
                Console.Write("Top Cards: ");
                for (int i = 0; i < inPlay.Count; i++) {
                    if (i == 3) { break; }
                    Console.Write("[" + inPlay[inPlay.Count-i-1].ToString() + "] ");
                }
                Console.WriteLine($"...({inPlay.Count} total cards in the pot)");
                int cardIndex = 0;
                int topVal;
                if (inPlay.Count == 0) {topVal = 0;}
                else {topVal = inPlay[inPlay.Count-1].val;}
                bool playable = false;
                foreach (Card c in players[turn].hand) {
                    Console.WriteLine(cardIndex + " - " + c.ToString() + " ");
                    cardIndex++;
                    if(c.val > topVal || c.val == 3 || c.val == 7 || c.val == 10 )
                    {
                        playable = true;
                    }
                }
                //future feature: players can play multiple cards if they have duplicates
                //future feature: keep player hand sorted


                //check if user has any playable cards

                //prompt user for which card to play
                if(playable)
                {
                    Console.WriteLine("Which card would you like to play?");
                    //grab top value of cards in play
                
                    int r = playSelection(players[turn].hand.Count, topVal, players[turn].hand);
                    //put this in an else
                    Card playedCard = players[turn].hand[r];
                    if(playedCard.val == 3)
                    {
                        players[turn].hand.Remove(playedCard);
                        foreach(Card c in inPlay)
                        {
                            players[(playCount + 1) % 3].hand.Add(c);
                        }
                        System.Console.WriteLine("Skipping {0}'s turn.", players[(playCount + 1) % 3].name);
                        System.Console.WriteLine(".... and they get all the cards HaHa!!.1");
                        inPlay.Clear();
                        playCount++;
                    }
                    else{
                        inPlay.Add(players[turn].discard(r));
                    }
                   
                    //redraw up to 3
                    while (players[turn].hand.Count < 3) {
                        if (d.cards.Count < 1) {
                            break;
                        }
                        players[turn].draw(d);
                    }
                }
                else
                {
                    System.Console.WriteLine("You can't play, have all the cards.");
                    foreach(Card c in inPlay)
                    {
                        players[turn].hand.Add(c);
                    }
                    inPlay.Clear();
                }
                //if hand is empty and deck is empty, play from castle

                //check if hand and castle are empty -- GAME OVER
                playCount++;
                if (playCount > 10) {
                    gameIsGood = false;
                }
            }
        
        }//end main

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
                System.Console.WriteLine("Uh, no. Try again.");
                return playSelection(numCards, topVal, playerCards);
            }
            else {
                return choice;
            }
        }
    }
}
