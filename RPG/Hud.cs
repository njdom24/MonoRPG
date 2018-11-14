using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    class Hud
    {
        private Texture2D chars;
        private Texture2D borders;
        private String message;
        private Point[] locations;
        private int width;
        private int height;
        private int offsetX;
        private int offsetY;
        private double timer;
        private int charCount;

        public Hud(String message, Texture2D chars, Texture2D borders)
        {
            charCount = 0;
            timer = 0;
            width = 18;
            height = 3;
            offsetX = (400 - width * 8) / 2;
            offsetY = 240 - (height + 2) * 8;
            this.chars = chars;
            this.borders = borders;
            this.message = message;
            //message = message.ToUpper();
            locations = new Point[message.Length];
            for(int i = 0; i < message.Length; i++)
            {
                char letter = message[i];
                if (letter >= 'A' && letter <= 'J')
                    locations[i] = new Point(8 * (letter - 'A'), 0);
                else if (letter >= 'K' && letter <= 'U')
                    locations[i] = new Point(8 * (letter - 'K'), 8);
                else if (letter >= 'V' && letter <= 'Z')
                    locations[i] = new Point(8 * (letter - 'V'), 16);
                else if (letter >= 'a' && letter <= 'e')
                    locations[i] = new Point(40 + 8 * (letter - 'a'), 16);
                else if (letter >= 'f' && letter <= 'o')
                    locations[i] = new Point(8 * (letter - 'f'), 24);
                else if (letter >= 'p' && letter <= 'z')
                    locations[i] = new Point(8 * (letter - 'p'), 32);
                else if (letter == '-')
                    locations[i] = new Point(0, 40);
                else if (letter == '"')
                    locations[i] = new Point(8, 40);
                else if (letter == '!')
                    locations[i] = new Point(16, 40);
                else if (letter == '?')
                    locations[i] = new Point(24, 40);
                else if (letter == '\'')
                    locations[i] = new Point(32, 40);
                else if (letter == ',')
                    locations[i] = new Point(40, 40);
                else if (letter == '.')
                    locations[i] = new Point(48, 40);
                else if (letter == '/')
                    locations[i] = new Point(56, 40);
                else if (letter == '<')
                    locations[i] = new Point(64, 40);
                else if (letter == '>')
                    locations[i] = new Point(72, 40);
                else if (letter >= '0' && letter <= '9')
                    locations[i] = new Point(8 * ((int)letter - (int)'0'), 48);
                else
                    locations[i] = new Point(80, 0);

            }
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= 0.1)
            {
                timer = 0;
                if (charCount < message.Length)
                    charCount++;
            }
        }
        private void DrawBlank(SpriteBatch sb)
        {
            //UL corner
            sb.Draw(borders, new Rectangle(offsetX, offsetY, 8, 8), new Rectangle(7 * 8, 0, 8, 8), Color.White);
            //BL corner
            sb.Draw(borders, new Rectangle(offsetX, offsetY + (height + 1) * 8, 8, 8), new Rectangle(2 * 8, 0, 8, 8), Color.White);
            //UR corner
            sb.Draw(borders, new Rectangle(offsetX + (width+1)*8, offsetY, 8, 8), new Rectangle(8 * 8, 0, 8, 8), Color.White);
            //BR corner
            sb.Draw(borders, new Rectangle(offsetX + (width+1)*8, offsetY + (height + 1) * 8, 8, 8), new Rectangle(3 * 8, 0, 8, 8), Color.White);
            //left&right
            for (int i = 0; i < height; i++)
            {
                sb.Draw(borders, new Rectangle(offsetX, offsetY + (i+1)*8, 8, 8), new Rectangle(4 * 8, 0, 8, 8), Color.White);
                sb.Draw(borders, new Rectangle(offsetX + (width+1)*8, offsetY + (i+1)*8, 8, 8), new Rectangle(5 * 8, 0, 8, 8), Color.White);
            }
            
            //top&bottom
            for (int i = 0; i < width; i++)
            {
                sb.Draw(borders, new Rectangle(offsetX + (i+1) * 8, offsetY, 8, 8), new Rectangle(6 * 8, 0, 8, 8), Color.White);
                sb.Draw(borders, new Rectangle(offsetX + (i+1) * 8, offsetY + (height+1)*8, 8, 8), new Rectangle(1 * 8, 0, 8, 8), Color.White);
            }
            //fill inside with blanks
            for(int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    sb.Draw(chars, new Rectangle(offsetX + (j + 1) * 8, offsetY + (i + 1) * 8, 8, 8), new Rectangle(80, 0, 8, 8), Color.White);
                }
            }
            
        }
        public Boolean isFinished()
        {
            return charCount == message.Length;
        }
 
        public void Draw(SpriteBatch sb)
        {
           
            DrawBlank(sb);
            int c = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (c < charCount)
                    {
                        if (message.ElementAt<char>(c) == '\n')
                        {
                            i++;
                            j = -1;
                        }
                        else
                            sb.Draw(chars, new Rectangle(offsetX + (j + 1) * 8, offsetY + (i + 1) * 8, 8, 8), new Rectangle(locations[c].X, locations[c].Y, 8, 8), Color.White);
                        c++;
                    }


            //sb.Draw(chars, new Rectangle(0, 0, 8, 8), new Rectangle(0, 0, 8, 8), Color.White);
            //sb.Draw(chars, new Rectangle(8, 0, 8, 8), new Rectangle(32, 0, 8, 8), Color.White);
        }
    }
}
