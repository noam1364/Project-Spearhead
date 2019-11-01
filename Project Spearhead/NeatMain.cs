using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using Project_Spearhead.MachineLearning.NEAT;
using static Global;

namespace Project_Spearhead
{
    class NeatMain : Game,IMainClass
    {
        #region data
        private int time = 1;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ///assets:
        Drawable bg;
        Drawable gameOverMenu;
        Manager neatManager;
        ///game objects:
        int spaceBetweenPillers;
        public static gameFlow flow;
        #endregion data
        #region ctor
        public NeatMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion ctor
        #region monogame methods

        protected override void Initialize()
        {
            base.Initialize();
            neatManager = new Manager();
            neatManager.Start();
            Global.game = this;
            flow = gameFlow.gameOn;
            graphics.PreferredBackBufferWidth = Global.winWidth;
            graphics.PreferredBackBufferHeight = Global.winHeight;
            graphics.ApplyChanges();
            Window.Title = "Desert Eagle";
            InputHandler.update();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Global.cm = Content;
            Global.sb = spriteBatch;
            Global.winHeight = 1080;
            Global.winWidth = 1920;

            Texture2D obstTex = Content.Load<Texture2D>("Textures/pillerSample");
            Texture2D bgTex = Content.Load<Texture2D>("Textures/background2");
            Texture2D gameOverTex = Content.Load<Texture2D>("Textures/gameOver");
            Obsticle.initStaticMembers(bgTex, obstTex);
            rock = new Obsticle[2];

            int gameOverMenuXpos = winWidth / 2 - gameOverTex.Width / 2;
            spaceBetweenPillers = winWidth / rock.Length;

            //jumpSound = Content.Load<SoundEffect>("Audio/wma/jumpSound");

            gameOverMenu = new Drawable(gameOverTex, new Vector2(gameOverMenuXpos, 200));
            bg = new Drawable(bgTex, new Vector2(0, 0));

            for(int i = 0;i < rock.Length;i++)
                rock[i] = new Obsticle(new Vector2(1920 + spaceBetweenPillers * i, 0));
        }

        protected override void Update(GameTime gameTime)
        {
            for(int i = 0;i < time;i++)
            {
                neatManager.Update();
                ReciveUserInput();
                foreach(Bird bird in neatManager.playerList)
                    bird.update();
                for(int j = 0;j < rock.Length;j++)
                    rock[j].movmentManager();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            bg.draw();
            for(int i = 0;i < rock.Length;i++)
                rock[i].draw();
            foreach(Bird b in neatManager.playerList)
                b.draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion monogame methods
        #region my methods
        public void restart()
        {
            for(int i = 0;i < rock.Length;i++)
                rock[i] = new Obsticle(new Vector2(1920 + spaceBetweenPillers * i, 0));
        }
        private void ReciveUserInput()
        {
            if(InputHandler.KeyStroke(Keys.Up))
                time += 10;
            if(Keyboard.GetState().IsKeyDown(Keys.Down) && time >= 11)
                time -= 10;
            InputHandler.update();
        }
        #endregion my methods
    }
}
