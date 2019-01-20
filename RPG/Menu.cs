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
	class Menu
	{
		private Text[] lines;
		private Texture2D textbox;
		private Texture2D background;
		private int offsetX, offsetY;
		private int width, height;
		private int spacingX, spacingY;
		private int rows, columns;
		private int deathOffX;
		private int prevIndexX, prevIndexY;
		private int highlightWidth;
		private int dividerHeight;

		private int cursorBob;
		private bool cursorRight;
		private double cursorTimer;

		private Selector selectorX, selectorY;
		private string[] items;
		//width 104
		public Menu(ContentManager contentManager)
		{
			textbox = contentManager.Load<Texture2D>("Textbox/Text");
			background = contentManager.Load<Texture2D>("HighlightColor");
			cursorRight = true;

			highlightWidth = 86;
			width = 25;//10
			height = 12;
			dividerHeight = (height+2) * 8 - 20;
			spacingX = 103;
			spacingY = 14;
			rows = 7;
			columns = 2;
			selectorX = new Selector(columns, true);
			selectorY = new Selector(rows, false);
			//automatically figure out height based on textHeight (coincidentally also 8) and spacing

			items = new string[] {	"Old Bat", "PSI Walnut", "Garden Band", "Feathery Charm", "Cookie", "Slingshot", "Local Soda",
									"Cheeseburger", "Belring Map" };
			lines = new Text[items.Length];


			for(int i = 0; i < items.Length; i++)
			{
				lines[i] = new Text(textbox, items[i], 51);//the 51 is deprecated
			}

			lines[0].SetColor(Color.Black);
		}

		public void Update(GameTime gameTime, KeyboardState prevState)
		{
			cursorTimer += gameTime.ElapsedGameTime.TotalSeconds;
			bool x = selectorX.Update(prevState);
			bool y = selectorY.Update(prevState);

			if(x || y)
			{
				if (selectorX.GetIndex() * rows + selectorY.GetIndex() < items.Length)
				{
					lines[prevIndexX * rows + prevIndexY].SetColor(Color.White);
					lines[selectorX.GetIndex() * rows + selectorY.GetIndex()].SetColor(Color.Black);
				}
				else
				{
					Console.WriteLine("KIRU: " + items.Length);
					Console.WriteLine(prevIndexX * rows + prevIndexY + 1);
					if (prevIndexX * rows + prevIndexY == items.Length - 1)//makes it go up if down is hit on an unfinished column
					{
						selectorY.SetIndex(0);
						lines[prevIndexX * rows + prevIndexY].SetColor(Color.White);
						lines[selectorX.GetIndex() * rows + selectorY.GetIndex()].SetColor(Color.Black);
					}
					else
					{
						selectorY.SetIndex(lines.Length % columns);
						lines[prevIndexX * rows + prevIndexY].SetColor(Color.White);
						lines[selectorX.GetIndex() * rows + selectorY.GetIndex()].SetColor(Color.Black);
						//selectorX.SetIndex(prevIndexX);
						//selectorY.SetIndex(prevIndexY);
					}
				}
			}

			if (cursorTimer > 0.15)
			{
				cursorTimer -= 0.15;

				if (cursorRight)
				{
					cursorBob++;
					if (cursorBob > 2)
						cursorRight = false;
				}
				else
				{
					cursorBob--;
					if (cursorBob < 1)
						cursorRight = true;
				}
			}


			prevIndexX = selectorX.GetIndex();
			prevIndexY = selectorY.GetIndex();
		}

		public void Draw(SpriteBatch sb)
		{
			DrawBlank(sb);
			Vector2 pos = new Vector2(offsetX + 16, offsetY + 7);

			sb.Draw(background, new Rectangle((int)pos.X - 3 + spacingX*selectorX.GetIndex(), (int)pos.Y + spacingY*selectorY.GetIndex() + 1, highlightWidth, 14), new Rectangle(0, 0, 1, 1), Color.White);
			//for loop it, also move it down with an offset and change 96 into something calculated
			sb.Draw(background, new Rectangle((int)(pos.X - 3 + (spacingX + highlightWidth)/2 + 1.51), (int)pos.Y + 3, 1, dividerHeight), new Rectangle(1, 0, 1, 1), Color.White);


			sb.Draw(textbox, new Rectangle((int)pos.X - 10 + cursorBob + spacingX*selectorX.GetIndex(), (int)pos.Y + spacingY * selectorY.GetIndex() + 3, 6, 9), new Rectangle(48, 96, 6, 9), Color.White);

			for (int i = 0; i < columns; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					if (i * rows + j < items.Length)
					{
						lines[i * rows + j].Draw(sb, pos);
						pos.Y += spacingY;
					}
				}
				pos.X += spacingX;
				pos.Y -= spacingY * rows;
			}

		}
		//48, 96
		private void DrawBlank(SpriteBatch sb)
		{
			//UL corner
			sb.Draw(textbox, new Rectangle(offsetX, offsetY, 8, 8), new Rectangle(0 + deathOffX, 112, 8, 8), Color.White);
			//BL corner
			sb.Draw(textbox, new Rectangle(offsetX, offsetY + (height + 1) * 8, 8, 8), new Rectangle(0 + deathOffX, 120, 8, 8), Color.White);
			//UR corner
			sb.Draw(textbox, new Rectangle(offsetX + (width + 1) * 8, offsetY, 8, 8), new Rectangle(8 + deathOffX, 112, 8, 8), Color.White);
			//BR corner
			sb.Draw(textbox, new Rectangle(offsetX + (width + 1) * 8, offsetY + (height + 1) * 8, 8, 8), new Rectangle(8 + deathOffX, 120, 8, 8), Color.White);

			//top&bottom
			//sb.Draw(textbox, new Rectangle(offsetX + (1) * 8, offsetY, width*8, 8), new Rectangle(16, 112, 1, 8), Color.White);
			//sb.Draw(textbox, new Rectangle(offsetX + (1) * 8, offsetY + (height + 1) * 8, width*8, 8), new Rectangle(17, 112, 1, 8), Color.White);
			for (int i = 0; i < width; i++)
			{
				sb.Draw(textbox, new Rectangle(offsetX + (i + 1) * 8, offsetY, 8, 8), new Rectangle(16 + deathOffX, 112, 8, 8), Color.White);//top
				sb.Draw(textbox, new Rectangle(offsetX + (i + 1) * 8, offsetY + (height + 1) * 8, 8, 8), new Rectangle(16 + deathOffX, 120, 8, 8), Color.White);//bottom
			}

			//left&right
			//sb.Draw(textbox, new Rectangle(offsetX, offsetY + (1) * 8, 8, 8*height), new Rectangle(20, 112, 8, 1), Color.White);//left
			//sb.Draw(textbox, new Rectangle(offsetX + (width+1)*8, offsetY + (1) * 8, 8, 8*height), new Rectangle(26, 112, 8, 1), Color.White);//right
			for (int i = 0; i < height; i++)
			{
				sb.Draw(textbox, new Rectangle(offsetX, offsetY + (i + 1) * 8, 8, 8), new Rectangle(24 + deathOffX, 112, 8, 8), Color.White);
				sb.Draw(textbox, new Rectangle(offsetX + (width + 1) * 8, offsetY + (i + 1) * 8, 8, 8), new Rectangle(24 + deathOffX, 120, 8, 8), Color.White);
			}


			//fill inside with black
			sb.Draw(textbox, new Rectangle(offsetX + 8, offsetY + 8, 8 * width, 8 * height), new Rectangle(7 + deathOffX, 119, 1, 1), Color.White);
			//215, 40
			//26 * 5
		}

		public void DeathMode(bool enabled)
		{
			if (enabled)
			{
				deathOffX = 32;
				//textColor = new Color(245, 139, 148);
			}
			else
			{
				deathOffX = 0;
				//textColor = Color.White;
			}
		}

		public void SetColor(Color color)
		{
			int index = selectorY.GetIndex();
			for(int i = 0; i < index; i++)
			{
				lines[i].SetColor(color);
			}
			for(int i = index; i < lines.Length; i++)
			{
				lines[i].SetColor(color);
			}
		}
	}
}
