using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Final
{
    struct Card
    {
        public string name;
        public float health, damage;
        public byte index;
        public Card(string name, float health, float damage, byte index)
        {
            this.name = name;
            this.health = health;
            this.damage = damage;
            this.index = index;
        }

        public Card[] Attack(Card enemy, Card[] field, byte index)
        {
            return enemy.TakeDamage(damage, field, index);
        }

        public Card[] TakeDamage(float damage, Card[] field, byte index)
        {
            field[index].health -= damage;
            if(field[index].health <= 0)
            {
                return Die(field, index);
            }
            return field;
        }

        private Card[] Die(Card[] field, byte index)
        {
            field[index] = new Card();
            return field;
        }
    }

    class Program
    {
        static Random random = new Random();

        static string pathToData = "Data.txt"; //Шлях до документа з картами

        static Card[] enemy = GenerateCards(4);
        static Card[] player = GenerateCards(4);
        static Card[] enemyField = new Card[4];
        static Card[] playerField = new Card[4];

        static void Main()
        {
            while (true)
            {
                Logic();
            }
        }

        static void Logic()
        {
            for (int i = 0; i < Math.Max(playerField.Length, enemyField.Length); i++)
            {
                if (playerField[i].name != null && enemyField[i].name != null)
                {
                    enemyField = playerField[i].Attack(enemyField[i], enemyField, (byte)i);
                    playerField = enemyField[i].Attack(playerField[i], playerField, (byte)i);
                }
            }
            ShowCards(enemy, true);
            ShowCards(enemyField, false);
            ShowCards(playerField, false);
            ShowCards(player, false);
            if (player.Length > 0)
            {
                byte curPlayerCard = Input((byte)player.Length);
                playerField = Place(playerField, player, curPlayerCard);
                player[curPlayerCard] = new Card();
            }
            //Thread.Sleep(random.Next(1, 5) * 1000);
            if (enemy.Length > 0)
            {
                byte curEnemyCard = GenerateRandomValue(enemy);
                enemyField = Place(enemyField, enemy, curEnemyCard);
                enemy[curEnemyCard] = new Card();
            }
            Console.Clear();
        }

        static byte GenerateRandomValue(Card[] enemy)
        {
            byte value;
            do value = (byte)random.Next(0, enemy.Length);
            while (enemy[value].name == null);
            return value;
        }

        static Card[] Place(Card[] field, Card[] source, byte curCard)
        {
            for (int i = 0; i < field.Length; i++)
            {
                if (field[i].name == null)
                {
                    field[i] = source[curCard];
                    break;
                }
            }
            return field;
        }

        static byte Input(byte length)
        {
            Console.Write("\b\rInput card index: ");
            string input = Console.ReadKey().KeyChar.ToString();
            if (!byte.TryParse(input, out byte output)) return Input(length);
            if (output > length - 1) return Input(length);
            //output = (byte)(length - 1);
            return output;
        }

        static Card[] GenerateCards(byte countOfCards)
        {
            Card[] cards = new Card[countOfCards];
            try
            {
                StreamReader reader = new StreamReader(pathToData); 
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    string[] cardsData = line.Split('/');
                    for (int i = 0; i < cards.Length; i++)
                    {
                        string[] cardsValues = cardsData[random.Next(cardsData.Length)].Split(',');
                        cards[i] = new Card(cardsValues[0], float.Parse(cardsValues[1]), float.Parse(cardsValues[2]), (byte)i);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            //for (int i = 0; i < countOfCards; i++) cards[i] = new Card("Card №" + (i + 1), random.Next(5, 10), random.Next(1, 5), (byte)i);
            return cards;
        }

        static void ShowCards(Card[] cards, bool hide)
        {
            foreach(Card card in cards) if (card.name != null) Console.Write("-----------\t"); 
            Console.WriteLine();
            foreach (Card card in cards)
            {
                if (card.name != null && !hide) Console.Write($"|\"{card.name}\"|\t");
                else if(hide) Console.Write($"|?????????|\t");
            }
            Console.WriteLine();
            foreach (Card card in cards)
            {
                if (card.name != null && !hide) Console.Write($"|Hp: {card.health}\t  |\t");
                else if (hide) Console.Write($"|?????????|\t");
            }
            Console.WriteLine();
            foreach (Card card in cards)
            {
                if (card.name != null && !hide) Console.Write($"|Dmg: {card.damage}\t  |\t");
                else if (hide) Console.Write($"|?????????|\t");
            }
            Console.WriteLine();
            foreach (Card card in cards) if (card.name != null) Console.Write("-----------\t");
            Console.WriteLine();
        }
    }
}
