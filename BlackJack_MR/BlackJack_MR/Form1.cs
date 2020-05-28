using BlackJack_MR.Common;
using BlackJack_MR.GameManager;
using BlackJack_MR.Players;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BlackJack_MR
{
    public partial class Blackjack : Form
    {
        public Player player;
        public Manager gm;
        private static int startXPos = -10;
        private static int dealerStartXPos = 285;
        private int playerCardXPos = startXPos;
        private int dealerCardXPos = dealerStartXPos;
        public List<PictureBox> playerCardsToDisplay;
        public List<PictureBox> dealerCards;
        string link = "Нажми 'Старт' для начала новой игры";

        public Blackjack()
        {
            InitializeComponent();
            gm = new Manager();
            player = new Player();
            gm.AddPlayer(player);
            playerCardsToDisplay = new List<PictureBox>();
            dealerCards = new List<PictureBox>();
            welcome.Text = "Добро пожаловать в мир Blackjack \nДля начала игры нажми кнопку 'Старт' \nДля ознакомления с правилами игры нажми кнопку '?'";
            splitButton.Hide();            
        }        

        private void DrawPlayerCard(Card card)
        {
            playerCardXPos += 30;
            PictureBox newCard = new PictureBox();
            Image img = Image.FromFile("../../card_images/" + card.Value + card.Suit + ".png");
            newCard.Image = img;
            newCard.Location = new System.Drawing.Point(playerCardXPos, 180);
            newCard.Name = "newCard";
            newCard.Size = new System.Drawing.Size(72, 99);
            this.Controls.Add(newCard);
            newCard.BringToFront();
            playerCardsToDisplay.Add(newCard);
        }

        private void DrawDealerCardNotShown(Card card)
        {
            dealerCardXPos -= 30;
            PictureBox blankCard = new PictureBox();
            Image blankImage = Image.FromFile("../../card_images/b1fv.png");
            blankCard.Image = blankImage;
            blankCard.Location = new System.Drawing.Point(dealerCardXPos, 12);
            blankCard.Name = "newCard";
            blankCard.Size = new System.Drawing.Size(72, 99);
            this.Controls.Add(blankCard);
            blankCard.BringToFront();
            dealerCards.Add(blankCard);
            DrawDealer(card);
        }

        private void DrawDealer(Card card)
        {
            dealerCardXPos -= 30;
            PictureBox newCard = new PictureBox();
            Image img = Image.FromFile("../../card_images/" + card.Value + card.Suit + ".png");
            newCard.Image = img;
            newCard.Location = new System.Drawing.Point(dealerCardXPos, 12);
            newCard.Name = "newCard";
            newCard.Size = new System.Drawing.Size(72, 99);
            this.Controls.Add(newCard);
            dealerCards.Add(newCard);            
        }    
               
        private void startButton_Click(object sender, EventArgs e)
        {
            resultLabel.Text = "";
            welcome.Visible = false;
            help.Visible = false;
            hitMeButton.Enabled = true;
            standButton.Enabled = true;
            RemoveCards(playerCardsToDisplay);
            RemoveCards(dealerCards);
            dealerCardXPos = dealerStartXPos;
            playerCardXPos = startXPos;
            List<Card> playerCards = player.ShowHand();
            gm.StartNewDeal();
            playerScore.Text = "Твой счет: " + player.GetScore();
            dealerScore.Text = "";
            DrawPlayerCard(playerCards[0]);
            DrawPlayerCard(playerCards[1]);
            DrawDealerCardNotShown(gm.DealerVisibleCard);

            if (playerCards[0].Value == playerCards[1].Value)
            {
                splitButton.Enabled = true;
            }
            if (player.GetScore() == 21)
            {
                resultLabel.Text = $"{link}";
                dealerScore.Text = "Счет дилера: " + gm.GetDealerScore();
                MessageBox.Show("Blackjack! Ты победил!");
                hitMeButton.Enabled = false;
                standButton.Enabled = false;
            }
        }

        private void RemoveCards(List<PictureBox> cardImages)
        {
            foreach (PictureBox box in cardImages)
            {
                this.Controls.Remove(box);
            }
        }        

        private void hitMeButton_Click_1(object sender, EventArgs e)
        {            
            if (!player.Busted)
            {
                player.HitMe(gm.DealCard());
                playerScore.Text = "Твой счет: " + player.GetScore();
                DrawPlayerCard(player.LastCard());                

                if (player.Busted)
                {
                    DrawDealer(gm.DealerLastCard);
                    hitMeButton.Enabled = false;
                    standButton.Enabled = false;
                    splitButton.Enabled = false;
                    dealerScore.Text = "Счет дилера: " + gm.GetDealerScore();
                    MessageBox.Show("Победа дилера!");
                    resultLabel.Text = $"{link}";
                }
            }
        }

        private void standButton_Click_1(object sender, EventArgs e)
        {
            dealerScore.Text = "Счет дилера: " + gm.GetDealerScore();
            playerScore.Text = "Твой счет: " + player.GetScore();
            dealerCardXPos = dealerStartXPos;
            RemoveCards(dealerCards);
            DrawDealer(gm.getDealerCards()[1]);
            DrawDealer(gm.getDealerCards()[0]);

            //TODO pause for 0.5 seconds so that the user can see the dealer getting cards
            while (gm.GetDealerScore() < 17)
            {
                gm.GiveDealerACard();
                dealerScore.Text = "Счет дилера: " + gm.GetDealerScore();
                DrawDealer(gm.DealerLastCard);
            }

            if (gm.GetDealerScore() > player.GetScore() && gm.GetDealerScore() < 22)
            {
                hitMeButton.Enabled = false;
                standButton.Enabled = false;
                splitButton.Enabled = false;                
                MessageBox.Show("Победа дилера!");
                resultLabel.Text = $"{link}";
            }
            else
            {
                hitMeButton.Enabled = false;
                standButton.Enabled = false;
                splitButton.Enabled = false;
                MessageBox.Show("Ты победил!");              
                resultLabel.Text = $"{link}";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string likk = "..\\..\\card_images\\BlackJackRules.html";            
            Process.Start(likk);
        }
    }
}
