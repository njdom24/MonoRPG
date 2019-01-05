using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    class Hud
    {
		private Texture2D textbox;
        private string[] messages;
        private Point[][] locations;
        private int width;
        private int height;
        private int offsetX;
        private int offsetY;
        private int posX;
        private int posY;
        private double timer;
        private int charCount;
        private int curMessage;
        private bool visible;
		private bool canClose;
		private Dictionary<char, int> charLengths;
		private int[] lengthRef;
		private int[][] indeces;
		private int lineLength;
		private int maxLines;
		private int linesToShift;
		private int linesShifted;
		private bool wait;

        public Hud(string[] message, ContentManager content, int width = 26, int height = 5, int posX = -1, int posY = -1, bool canClose = true, int maxLines = 3)
        {
			lengthRef = new int[] { 2, 2, 3, 2, 5, 9, 7, 2, 3, 3, 3, 5, 2, 2, 2, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 2, 3, 3, 5, 3, 4, 5, 6, 5, 5, 5, 4, 4, 5, 5, 1, 4, 5, 4, 7, 5, 5, 5, 5, 5, 5, 5, 5, 6, 7, 5, 5, 4, 5, 4, 6, 4, 5, 1, 4, 4, 4, 4, 4, 3, 4, 4, 1, 2, 4, 1, 7, 4, 4, 4, 4, 3, 4, 3, 4, 5, 7, 4, 4, 4, 2, 5, 2, 6, 7, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
			indeces = new int[message.Length][];
			for(int i = 0; i < indeces.Length; i++)
				indeces[i] = new int[message[i].Length];
			lineLength = 0;
			this.maxLines = maxLines - 2;
			linesShifted = 0;
			linesToShift = 0;
			wait = false;
			this.canClose = canClose;
            this.posX = posX;
            this.posY = posY;
            visible = true;
            curMessage = 0;
            charCount = 0;
            timer = 0;
            this.width = width;
            this.height = height;

            if (posX == -1)
                offsetX = (400 - (width + 2) * 8) / 2;
            else
                offsetX = posX;
			if (posY == -1)
				offsetY = 240 - (height + 2) * 8 - 4;
			else
				offsetY = posY;

			this.textbox = content.Load<Texture2D>("Textbox/Text");
            this.messages = message;
            //message = message.ToUpper();
            locations = new Point[messages.Length][];//[message.Length];
            for (int i = 0; i < locations.Length; i++)
                locations[i] = new Point[messages[i].Length];
            for(int i = 0; i < messages.Length; i++)
                for(int j = 0; j < messages[i].Length; j++)
                {
                    char letter = message[i][j];
					int line, off;
					if (letter != '\n')
					{
						//Determinesthe sprite's position based on its distance from the first character in the spritesheet, the space
						line = (letter - ' ') / 16;
						off = (letter - ' ') % 16;
						locations[i][j] = new Point(16 * off, 16 * line);
						indeces[i][j] = lengthRef[(int)(letter - ' ')] + 1;// + indeces[i][j-1] + 1;
						if (j > 0)//keeps the offset cumulative
							indeces[i][j] += indeces[i][j - 1];
					}
				}
        }

		public void finishMessage()
		{
			//curMessage = messages.Length - 1;
			charCount = messages[curMessage].Length;
			//visible = !canClose;
		}

		public void finishText()
		{
			curMessage = messages.Length - 1;
			charCount = messages[curMessage].Length;
			visible = !canClose;
		}

		public void Update(GameTime gameTime, KeyboardState prevState)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevState.IsKeyDown(Keys.Space))
			{
				if (wait)
				{
					wait = false;
					if (linesShifted > maxLines)
						linesToShift++;

					linesShifted++;
				}

				if (true)
				{
					if (isFinished())// && curMessage < messages.Length - 1)
					{
						if (curMessage < messages.Length - 1)
						{
							//advance message
							curMessage++;
							charCount = 0;
							lineLength = 0;
							linesToShift = 0;
							linesShifted = 0;
						}
						else
						{
							//close textbox
							visible = !canClose;
						}
					}
					else
					{
						if (charCount < messages[curMessage].Length)
						{
							//skip text
							//charCount = messages[curMessage].Length;

							//CHANGE TO SKIP LINE
						}
					}
				}
				//spacePressedLastFrame = true;
			}
			else
			{
				//spacePressedLastFrame = false;
				timer += gameTime.ElapsedGameTime.TotalSeconds;
				if (timer >= 0.04)
				{
					timer = 0;
					int len = messages[curMessage].Length;
					if (!wait && charCount < len)
					{
						if (messages[curMessage].ElementAt(charCount) == '\n')
						{
							if (charCount + 1 != len && messages[curMessage].ElementAt(charCount + 1) == '@')
							{
								wait = true;
							}
							else
							{
								if (linesShifted > maxLines)
									linesToShift++;

								linesShifted++;
							}
						}
						charCount++;
					}
					
				}
			}
		}
        private void DrawBlank(SpriteBatch sb)
        {
            //UL corner
            sb.Draw(textbox, new Rectangle(offsetX, offsetY, 8, 8), new Rectangle(0, 112, 8, 8), Color.White);
            //BL corner
            sb.Draw(textbox, new Rectangle(offsetX, offsetY + (height + 1) * 8, 8, 8), new Rectangle(0, 120, 8, 8), Color.White);
            //UR corner
            sb.Draw(textbox, new Rectangle(offsetX + (width+1)*8, offsetY, 8, 8), new Rectangle(8, 112, 8, 8), Color.White);
            //BR corner
            sb.Draw(textbox, new Rectangle(offsetX + (width+1)*8, offsetY + (height + 1) * 8, 8, 8), new Rectangle(8, 120, 8, 8), Color.White);

			//top&bottom
			//sb.Draw(textbox, new Rectangle(offsetX + (1) * 8, offsetY, width*8, 8), new Rectangle(16, 112, 1, 8), Color.White);
			//sb.Draw(textbox, new Rectangle(offsetX + (1) * 8, offsetY + (height + 1) * 8, width*8, 8), new Rectangle(17, 112, 1, 8), Color.White);
			for (int i = 0; i < width; i++)
			{
				sb.Draw(textbox, new Rectangle(offsetX + (i + 1) * 8, offsetY, 8, 8), new Rectangle(16, 112, 8, 8), Color.White);//top
				sb.Draw(textbox, new Rectangle(offsetX + (i + 1) * 8, offsetY + (height + 1) * 8, 8, 8), new Rectangle(16, 120, 8, 8), Color.White);//bottom
			}

			//left&right
			//sb.Draw(textbox, new Rectangle(offsetX, offsetY + (1) * 8, 8, 8*height), new Rectangle(20, 112, 8, 1), Color.White);//left
			//sb.Draw(textbox, new Rectangle(offsetX + (width+1)*8, offsetY + (1) * 8, 8, 8*height), new Rectangle(26, 112, 8, 1), Color.White);//right
			for (int i = 0; i < height; i++)
			{
				sb.Draw(textbox, new Rectangle(offsetX, offsetY + (i + 1) * 8, 8, 8), new Rectangle(24, 112, 8, 8), Color.White);
				sb.Draw(textbox, new Rectangle(offsetX + (width + 1) * 8, offsetY + (i + 1) * 8, 8, 8), new Rectangle(24, 120, 8, 8), Color.White);
			}


			//fill inside with black
			sb.Draw(textbox, new Rectangle(offsetX + 8, offsetY + 8, 8 * width, 8 * height), new Rectangle(1, 97, 14, 14), Color.White);
			//215, 40
			//26 * 5
		}
        public bool isFinished()
        {
            return charCount == messages[curMessage].Length;
        }
 
        public void Draw(SpriteBatch sb)
        {
            if (visible)
            {
                DrawBlank(sb);
				char letter;
				int lineCount = 0;

				for(int i = 0; i < charCount; i++)
				{
					letter = messages[curMessage].ElementAt<char>(i);
					if (letter == '\n')
					{
						lineCount++;
					}
					else if(lineCount >= linesToShift)
					{
						int lineOffset = 0;
						if (i > 0)
							lineOffset = indeces[curMessage][i - 1];

						sb.Draw(textbox, new Rectangle(offsetX + 8 + lineOffset * 1, offsetY + (lineCount - linesToShift) * 12 + 8, 16, 16), new Rectangle(locations[curMessage][i].X, locations[curMessage][i].Y, 16, 16), Color.White);
					}
				}
            }
        }

        public bool messageComplete()
        {
            return !visible;
        }

		public int getHeight()
		{
			return height * 8;
		}

		public bool IsWaiting()
		{
			return wait;
		}
    }
}
