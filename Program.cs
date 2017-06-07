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
            int t = 0;
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
                Console.Clear();
                Console.WriteLine($"Pass the computer to {players[(t + 1) % 3].name}. {players[(t + 1) % 3].name}, press enter when ready.");
                Console.ReadLine();
                t++;
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
                Console.WriteLine("*********************************************************************************************");
                Console.Write("Top Cards: ");
                for (int i = 0; i < inPlay.Count; i++) {
                    if (i == 3) { break; }
                    Console.Write(inPlay[inPlay.Count-i-1].ToString() + " ");
                }
                Console.WriteLine("...");
                Console.Write($"{inPlay.Count} total cards in the pot");
                if (d.cards.Count > 0) {
                    Console.Write($"            Cards Left in Draw Pile: {d.cards.Count}");
                }
                else { Console.Write("                                         No Cards Left in Draw Pile");}
                Console.WriteLine();
                Console.WriteLine("*********************************************************************************************");
                int topVal;
                if (inPlay.Count == 0) {topVal = 0;}
                else {topVal = inPlay[inPlay.Count-1].val;}
                //double check for 7's on top
                if (topVal == 7 && inPlay.Count > 1) {topVal = inPlay[inPlay.Count-2].val;}
                else if (topVal == 7 && inPlay.Count == 1) {topVal = 0;}
                bool handPlayable = is_playable(players[turn].hand, topVal, false);
                //future feature: players can play multiple cards if they have duplicates
                //future feature: keep player hand sorted


                //check if user has any playable cards

                //prompt user for which card to play from hand
                if(handPlayable)
                {
                    Console.WriteLine("Which card would you like to play?");
                    //grab top value of cards in play
                
                    int r = playSelection(players[turn].hand.Count, topVal, players[turn].hand);
                    //put this in an else
                    Card playedCard = players[turn].hand[r];
                    if(playedCard.val == 3) //next player takes the pot and their turn is skipped
                    {
                        players[turn].hand.Remove(playedCard);
                        foreach(Card c in inPlay)
                        {
                            players[(playCount + 1) % 3].hand.Add(c);
                        }
                        System.Console.WriteLine("Skipping {0}'s turn.", players[(playCount + 1) % 3].name);
                        System.Console.WriteLine(".... and they get all the cards HaHa!!.1");
                        players[(playCount + 1) % 3].sortHand();
                        inPlay.Clear();
                        playCount++;
                    }
                    else if(playedCard.val == 10) //whole pot is removed from play
                    {
                        inPlay.Add(players[turn].discard(r));
                        inPlay.Clear();
                        Console.WriteLine("10's clear the pot!");
                    }
                    else //includes 7's
                    { 
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
                else if (players[turn].hand.Count > 0)
                {
                    System.Console.WriteLine("You can't play, have all the cards.");
                    foreach(Card c in inPlay)
                    {
                        players[turn].hand.Add(c);
                    }
                    players[turn].sortHand();
                    inPlay.Clear();
                }
                //if hand is empty and deck is empty, play from castle
                else if (players[turn].hand.Count < 1 && d.cards.Count == 0 && players[turn].castle.Count > 3)
                {
                    bool vCastlePlayable = is_playable(players[turn].castle, topVal, true);
                    if (vCastlePlayable)
                    {
                        Console.WriteLine("Which castle card would you like to play?");
                        int r = playSelection(players[turn].castle.Count,3);
                        Card playedCard = players[turn].castle[r];
                        if(playedCard.val == 3) //next player takes the pot and their turn is skipped
                        {
                            players[turn].castle.Remove(playedCard);
                            foreach(Card c in inPlay)
                            {
                                players[(playCount + 1) % 3].hand.Add(c);
                            }
                            System.Console.WriteLine("Skipping {0}'s turn.", players[(playCount + 1) % 3].name);
                            System.Console.WriteLine(".... and they get all the cards HaHa!!.1");
                            players[(playCount + 1) % 3].sortHand();
                            inPlay.Clear();
                            playCount++;
                        }
                        else if(playedCard.val == 10) //whole pot is removed from play
                        {
                            players[turn].castle.Remove(playedCard);
                            inPlay.Clear();
                            Console.WriteLine("10's clear the pot!");
                        }
                        else //includes 7's
                        { 
                            inPlay.Add(playedCard);
                            players[turn].castle.Remove(playedCard);
                        }
                    }
                    else //castle is not playable
                    {
                        System.Console.WriteLine("You can't play, have all the cards.");
                        foreach(Card c in inPlay)
                        {
                            players[turn].hand.Add(c);
                        }
                        players[turn].sortHand();
                        inPlay.Clear();
                    }
                }
                //time to play from the invisible hand!
                else if (players[turn].hand.Count < 1 && d.cards.Count == 0 && players[turn].castle.Count < 4) 
                {
                    //fix playing special cards from hand, discarding
                    Console.WriteLine($"Pick a card from your hidden castle....ooOooOooOOh. Pick from 0 to {players[turn].castle.Count-1}");
                    int r = playSelection(players[turn].castle.Count);
                    List<Card> invisibleSelection = new List<Card>(){players[turn].castle[r]};
                    if (is_playable(invisibleSelection,topVal,true))
                    {
                        inPlay.Add(players[turn].castle[r]);
                        players[turn].castle.RemoveAt(r);
                    }
                    else
                    {
                        System.Console.WriteLine("Oops! That card didn't work.");
                        players[turn].hand.Add(players[turn].castle[r]);
                        players[turn].castle.RemoveAt(r);
                        foreach(Card c in inPlay)
                        {
                            players[turn].hand.Add(c);
                        }
                        players[turn].sortHand();
                        inPlay.Clear();
                    }
                    
                }
            
                //check if hand and castle are empty -- GAME OVER
                if (players[turn].hand.Count == 0 && players[turn].castle.Count == 0) {
                    System.Console.WriteLine("YOU WIIIIINNNNN!!!!!");
                    gameIsGood = false;
                }
                else {
                    Console.Clear();
                    Console.WriteLine($"Pass the computer to {players[(playCount + 1) % 3].name}.");
                    Console.ReadLine();
                }
                playCount++;
            }
        
        }//end main

        public static int playSelection(int numCards, int min = 0) {
            string response = Console.ReadLine();
            int choice;
            //if invalid choice
            if(!int.TryParse(response, out choice) || choice >= numCards || choice < min) {
                System.Console.WriteLine("I'm sorry but that wasn't a recognizable choice. Please try again.");
                return playSelection(numCards);
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
        public static bool is_playable(List<Card> cards, int topVal, bool is_castle)
        {
            bool playable = false;
            int cardIndex = 0;
            if (is_castle && cards.Count > 3) 
            {
                cards = cards.GetRange(3,cards.Count-3);
                cardIndex = 3;
            }
            foreach (Card c in cards) {
                    Console.WriteLine(cardIndex + " - " + c.ToString() + " ");
                    cardIndex++;
                    if(c.val >= topVal || c.val == 3 || c.val == 7 || c.val == 10 )
                    {
                        playable = true;
                    }
                }
            return playable;
        }
    }
}
