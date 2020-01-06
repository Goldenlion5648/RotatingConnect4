using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RotatingConnect4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kb, oldkb;
        MouseState mouseState, oldmouseState;
        Point mousePos;
        Random rand = new Random();
        SpriteFont customfont, titleAndEndFont;

        Character[,] backgroundCharacterGrid;
        Character[,] backgroundCharacterGridTemp;
        Character backgroundOutline;

        Character arrowIndicator;

        List<Character> player0Balls = new List<Character>();
        List<Character> player1Balls = new List<Character>();

        int gameClock = 0;
        int cooldownTimer = 0;

        int gridXDimension = 7;
        int gridYDimension = 7;

        int selectedColumn = 1;

        int protocol = -1;

        int screenWidth, screenHeight;

        int playerTurnIndicator = 0;
        int randomDecider = 0;

        int autoPlayCooldown = 0;

        bool isAiTurn = false;
        bool isAiEnabled = true;

        bool autoPlay = false;

        bool isRotationEnabled = true;
        bool hasRotated = false;

        bool aiRotatedLastTurn = false;

        bool rotatedBoardLastTurn = false;

        int previousColumn = -1;

        int priority = 0;

        int totalTilesOnBoard = 0;

        int lowerPriorityMove = -1;

        bool hasDoneOneTimeCode = false;

        bool shouldPlayAnimation = false;
        bool isCaughtUpInGravity = true;

        bool wasRandom = false;
        bool checkedBlue = false;

        bool redWins = false;
        bool blueWins = false;

        //bool hasMadeMoveYet = false;

        List<int> blackListedColumns = new List<int>();



        #region gamestateThings

        enum gameState
        {

            titleScreen, gamePlay, options

        }


        gameState state = gameState.gamePlay;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //this.graphics.PreferredBackBufferWidth = 840;
            //this.graphics.PreferredBackBufferHeight = 630;

            this.graphics.PreferredBackBufferWidth = 840;
            this.graphics.PreferredBackBufferHeight = 630;

            screenWidth = this.graphics.PreferredBackBufferWidth;
            screenHeight = this.graphics.PreferredBackBufferHeight;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            customfont = Content.Load<SpriteFont>("customfont");
            titleAndEndFont = Content.Load<SpriteFont>("titleAndEndFont");

            backgroundCharacterGrid = new Character[gridYDimension, gridXDimension];
            backgroundCharacterGridTemp = new Character[gridYDimension, gridXDimension];

            arrowIndicator = new Character(Content.Load<Texture2D>("arrow"), new Rectangle(0, 0, 80, 80));

            //int offsetX = 90;
            //int offsetY = 60;

            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    backgroundCharacterGrid[y, x] = new Character(Content.Load<Texture2D>("buttonOutline"),
                         new Rectangle(x * (screenWidth / gridXDimension), y * (screenHeight / gridYDimension),
                         (screenWidth / gridXDimension), (screenHeight / gridYDimension)));

                    backgroundCharacterGridTemp[y, x] = new Character(Content.Load<Texture2D>("buttonOutline"),
                         new Rectangle(x * (screenWidth / gridXDimension), y * (screenHeight / gridYDimension),
                         (screenWidth / gridXDimension), (screenHeight / gridYDimension)));


                }

            }

            backgroundOutline = new Character(Content.Load<Texture2D>("buttonOutline"), new Rectangle(0, 0, screenWidth, screenHeight));


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            kb = Keyboard.GetState();
            mouseState = Mouse.GetState();
            mousePos = new Point(mouseState.X, mouseState.Y);



            switch (state)
            {

                case gameState.titleScreen:
                    titleScreen();
                    break;
                case gameState.gamePlay:

                    gamePlay(gameTime);
                    break;
                case gameState.options:

                    options();

                    break;

            }
            //changeColor();
            oldkb = kb;
            oldmouseState = mouseState;
            base.Update(gameTime);
        }

        private void titleScreen()
        {


        }

        private void options()
        {


        }

        private void gamePlay(GameTime gameTime)
        {
            if (hasDoneOneTimeCode == false)
            {

            }

            if ((blueWins || redWins) && autoPlay)
            {
                if (autoPlayCooldown == 0)
                {
                    autoPlayCooldown = gameClock;
                }
                if (gameClock - autoPlayCooldown > 300)
                {
                    resetGame();
                }
            }


            userControls();
            aiMoves();
            playAnimation();
            //rotateBoard();
            arrowMovement();
            checkWins();
            checkDraws();

            //testRect.rotate(0.05f, spriteBatch);

            gameClock++;
        }

        private void userControls()
        {
            //|| (kb.IsKeyDown(Keys.Left) && gameClock % 20 == 0)
            if ((kb.IsKeyDown(Keys.Left) && oldkb.IsKeyUp(Keys.Left)) || (kb.IsKeyDown(Keys.A) && oldkb.IsKeyUp(Keys.A)))
            {
                if (selectedColumn > 1)
                {
                    selectedColumn -= 1;
                }
            }
            //|| (kb.IsKeyDown(Keys.Right) && gameClock % 20 == 0)
            if ((kb.IsKeyDown(Keys.Right) && oldkb.IsKeyUp(Keys.Right)) || (kb.IsKeyDown(Keys.D) && oldkb.IsKeyUp(Keys.D)))
            {
                if (selectedColumn < gridXDimension)
                {
                    selectedColumn += 1;
                }
            }

            if ((kb.IsKeyDown(Keys.R) && oldkb.IsKeyUp(Keys.R)))
            {
                resetGame();
            }

            if ((kb.IsKeyDown(Keys.J) && oldkb.IsKeyUp(Keys.J)))
            {
                autoPlay = !autoPlay;
            }

            if ((kb.IsKeyDown(Keys.G) && oldkb.IsKeyUp(Keys.G)))
            {
                shouldPlayAnimation = !shouldPlayAnimation;
            }

            if ((kb.IsKeyDown(Keys.P) && oldkb.IsKeyUp(Keys.P)))
            {
                isRotationEnabled = !isRotationEnabled;
            }

            if ((kb.IsKeyDown(Keys.I) && oldkb.IsKeyUp(Keys.I)))
            {

                isAiEnabled = !isAiEnabled;

            }

            if ((kb.IsKeyDown(Keys.T) && oldkb.IsKeyUp(Keys.T)))
            {
                if (totalTilesOnBoard < 2 && shouldPlayAnimation == false)
                {
                    isAiEnabled = !isAiEnabled;
                }
            }

            if (kb.IsKeyDown(Keys.O) && oldkb.IsKeyUp(Keys.O) && isRotationEnabled && isAiTurn == false && totalTilesOnBoard >= 4 && rotatedBoardLastTurn == false &&
                (redWins == false && blueWins == false))
            {
                rotateBoard();
                if (playerTurnIndicator == 0)
                {
                    playerTurnIndicator = 1;
                }
                else
                {
                    playerTurnIndicator = 0;
                }

                shouldPlayAnimation = true;
                rotatedBoardLastTurn = true;
                if (isAiEnabled)
                    isAiTurn = true;
            }

            if (kb.IsKeyDown(Keys.Space) && oldkb.IsKeyUp(Keys.Space) && shouldPlayAnimation == false && blueWins == false && redWins == false && isAiTurn == false)
            {

                if (backgroundCharacterGrid[0, selectedColumn - 1].getIsOccupied() == false)
                {
                    if (playerTurnIndicator == 0)
                    {
                        backgroundCharacterGrid[0, selectedColumn - 1].setIsBlueTile();
                        playerTurnIndicator = 1;
                        shouldPlayAnimation = true;

                        previousColumn = selectedColumn - 1;
                        totalTilesOnBoard += 1;
                        hasRotated = false;
                        rotatedBoardLastTurn = false;

                        if (isAiEnabled)
                            isAiTurn = true;

                    }
                    else if (playerTurnIndicator == 1)
                    {
                        backgroundCharacterGrid[0, selectedColumn - 1].setIsRedTile();
                        shouldPlayAnimation = true;
                        playerTurnIndicator = 0;
                        previousColumn = selectedColumn - 1;
                        totalTilesOnBoard += 1;
                        hasRotated = false;
                        rotatedBoardLastTurn = false;

                        if (isAiEnabled)
                            isAiTurn = true;




                    }


                }
            }

        }

        private void checkWins()
        {
            if (shouldPlayAnimation == false)
            {
                for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                    {
                        if (j < gridXDimension - 3)
                        {
                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i, j + 1].getIsRedTile() &&
                                backgroundCharacterGrid[i, j + 2].getIsRedTile() && backgroundCharacterGrid[i, j + 3].getIsRedTile())
                            {
                                redWins = true;
                            }
                            if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i, j + 1].getIsBlueTile() &&
                                backgroundCharacterGrid[i, j + 2].getIsBlueTile() && backgroundCharacterGrid[i, j + 3].getIsBlueTile())
                            {
                                blueWins = true;
                            }
                        }

                        if (i < gridYDimension - 3)
                        {
                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i + 1, j].getIsRedTile() &&
                                backgroundCharacterGrid[i + 2, j].getIsRedTile() && backgroundCharacterGrid[i + 3, j].getIsRedTile())
                            {
                                redWins = true;
                            }

                            if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i + 1, j].getIsBlueTile() &&
                                backgroundCharacterGrid[i + 2, j].getIsBlueTile() && backgroundCharacterGrid[i + 3, j].getIsBlueTile())
                            {
                                blueWins = true;
                            }

                        }

                        if (i < gridYDimension - 3 && j < gridXDimension - 3)
                        {
                            if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i + 1, j + 1].getIsBlueTile() &&
                                    backgroundCharacterGrid[i + 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i + 3, j + 3].getIsBlueTile())
                            {
                                blueWins = true;
                            }

                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i + 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i + 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i + 3, j + 3].getIsRedTile())
                            {
                                redWins = true;
                            }

                        }

                        if (i > 2 && j < gridXDimension - 3)
                        {
                            if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i - 1, j + 1].getIsBlueTile() &&
                                    backgroundCharacterGrid[i - 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i - 3, j + 3].getIsBlueTile())
                            {
                                blueWins = true;
                            }

                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i - 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i - 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i - 3, j + 3].getIsRedTile())
                            {
                                redWins = true;
                            }

                        }

                    }
                }

            }


        }

        private void aiMoves()
        {

            if (isAiTurn && shouldPlayAnimation == false && !redWins && !blueWins)
            {
                wasRandom = false;
                blackListedColumns.Clear();
                randomDecider = -1;
                protocol = -1;
                lowerPriorityMove = -1;
                for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                    {
                        #region redAI

                        if (j < gridXDimension - 3)
                        {
                            //blank red red red in a line
                            if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsRedTile() &&
                                backgroundCharacterGrid[i, j + 2].getIsRedTile() && backgroundCharacterGrid[i, j + 3].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                                {
                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 1;
                                    break;
                                }
                            }
                            //red red red blank in a line
                            if (backgroundCharacterGrid[i, j + 3].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsRedTile() &&
                            backgroundCharacterGrid[i, j + 2].getIsRedTile() && backgroundCharacterGrid[i, j].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j + 3].getIsOccupied()))
                                {
                                    randomDecider = j + 3;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 12;
                                    break;
                                }
                            }
                            //red blank red red in a line
                            if (backgroundCharacterGrid[i, j + 1].getIsOccupied() == false && backgroundCharacterGrid[i, j + 3].getIsRedTile() &&
                                backgroundCharacterGrid[i, j + 2].getIsRedTile() && backgroundCharacterGrid[i, j].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j + 1].getIsOccupied()))
                                {
                                    randomDecider = j + 1;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 13;
                                    break;
                                }
                            }
                            //red red blank red in a line
                            if (backgroundCharacterGrid[i, j + 2].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsRedTile() &&
                                backgroundCharacterGrid[i, j + 3].getIsRedTile() && backgroundCharacterGrid[i, j].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j + 2].getIsOccupied()))
                                {
                                    randomDecider = j + 2;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 14;
                                    break;
                                }
                            }
                        }
                        if (i < gridYDimension - 3)
                        {
                            //blank red red red going down
                            if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j].getIsRedTile() &&
                                backgroundCharacterGrid[i + 2, j].getIsRedTile() && backgroundCharacterGrid[i + 3, j].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                                {
                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 15;
                                    break;
                                }
                            }
                        }
                        if (i < gridYDimension - 3 && j < gridXDimension - 3)
                        {
                            //red red red blank in downward diagonal
                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i + 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i + 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i + 3, j + 3].getIsOccupied() == false)
                            {
                                if (i == gridYDimension - 1 || (i + 4 < gridYDimension && backgroundCharacterGrid[i + 4, j + 3].getIsOccupied()))
                                {
                                    randomDecider = j + 3;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 17;
                                    break;
                                }
                            }
                            //blank red red red downward diagonal
                            if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i + 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i + 3, j + 3].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                                {
                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 18;
                                    break;
                                }
                            }
                            ////blank red red blank in upward diagonal
                            //if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j + 1].getIsRedTile() &&
                            //        backgroundCharacterGrid[i + 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i + 3, j + 3].getIsOccupied() == false)
                            //{
                            //    if (i == gridYDimension - 1 || (i + 4 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                            //    {
                            //        randomDecider = j;
                            //        i = backgroundCharacterGrid.GetLength(0);
                            //        j = backgroundCharacterGrid.GetLength(1);
                            //        protocol = 19;
                            //        break;
                            //    }
                            //}

                            //blank red red blank in downward diagonal
                            if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i + 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i + 3, j + 3].getIsOccupied() == false)
                            {
                                if ((i + 4 < gridYDimension && j + 4 < gridXDimension && backgroundCharacterGrid[i + 4, j + 4].getIsOccupied() == false))
                                {
                                    randomDecider = j + 3;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 19;
                                    break;
                                }

                                if ((i + 4 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                                {
                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 19;
                                    break;
                                }
                            }
                            //red red blank red in downward diagonal
                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i + 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i + 2, j + 2].getIsOccupied() == false && backgroundCharacterGrid[i + 3, j + 3].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 2 < gridYDimension && backgroundCharacterGrid[i + 3, j + 2].getIsOccupied()))
                                {
                                    randomDecider = j + 2;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 21;
                                    break;
                                }
                            }


                        }
                        if (i > 2 && j < gridXDimension - 3)
                        {

                            //red red blank red in upward diagonal
                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i - 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i - 2, j + 2].getIsOccupied() == false && backgroundCharacterGrid[i - 3, j + 3].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i - 2 < gridYDimension && backgroundCharacterGrid[i - 1, j + 2].getIsOccupied()))
                                {
                                    randomDecider = j + 2;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 31;
                                    break;
                                }
                            }
                            //blank red red red in upward diagonal
                            if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i - 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i - 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i - 3, j + 3].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                                {
                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 33;
                                    break;
                                }
                            }
                            //red blank red red in upward diagonal
                            if (backgroundCharacterGrid[i - 1, j + 1].getIsOccupied() == false && backgroundCharacterGrid[i, j].getIsRedTile() &&
                                    backgroundCharacterGrid[i - 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i - 3, j + 3].getIsRedTile())
                            {
                                if (i == gridYDimension - 1 || (i < gridYDimension && backgroundCharacterGrid[i, j + 1].getIsOccupied()))
                                {
                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 62;
                                    break;
                                }
                            }

                            //red red red blank in upward diagonal
                            if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i - 1, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i - 2, j + 2].getIsRedTile() && backgroundCharacterGrid[i - 3, j + 3].getIsOccupied() == false)
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i - 2, j + 3].getIsOccupied()))
                                {
                                    randomDecider = j + 3;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 36;
                                    break;
                                }
                            }
                        }

                        //if (j + 3 < gridXDimension)
                        //{
                        //    //blank red red blank
                        //    if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsRedTile() &&
                        //            backgroundCharacterGrid[i, j + 2].getIsRedTile() && backgroundCharacterGrid[i, j + 3].getIsOccupied() == false)
                        //    {
                        //        if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                        //        {
                        //            randomDecider = j;
                        //            i = backgroundCharacterGrid.GetLength(0);
                        //            j = backgroundCharacterGrid.GetLength(1);
                        //            protocol = 39;
                        //            break;
                        //        }
                        //    }
                        //}

                        if (j + 3 < gridXDimension)
                        {
                            if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsRedTile() &&
                                    backgroundCharacterGrid[i, j + 2].getIsRedTile() && backgroundCharacterGrid[i, j + 3].getIsOccupied() == false)
                            {
                                if (i == gridYDimension - 1 || (i + 1 < gridYDimension && backgroundCharacterGrid[i + 1, j].getIsOccupied()))
                                {
                                    lowerPriorityMove = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    break;
                                }
                            }
                        }
                    }

                }
                #endregion

                if (randomDecider == -1)
                {

                    for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
                    {
                        for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                        {
                            checkedBlue = true;
                            #region blue
                            #region horizontal
                            if (j < gridXDimension - 3)
                            {
                                //blank blue blue blue in a line
                                if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsBlueTile() &&
                                    backgroundCharacterGrid[i, j + 2].getIsBlueTile() && backgroundCharacterGrid[i, j + 3].getIsBlueTile())
                                {
                                    if (i == gridYDimension - 1)
                                    {

                                        randomDecider = j;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 7;
                                        break;

                                    }

                                    if (i + 1 < gridYDimension)
                                    {
                                        if (backgroundCharacterGrid[i + 1, j].getIsOccupied())
                                        {
                                            randomDecider = j;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 7;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j) == false)
                                                blackListedColumns.Add(j);
                                        }
                                    }
                                }
                                //blue blue blue blank in a line
                                if (backgroundCharacterGrid[i, j + 3].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsBlueTile() &&
                                    backgroundCharacterGrid[i, j + 2].getIsBlueTile() && backgroundCharacterGrid[i, j].getIsBlueTile())
                                {
                                    if (i + 1 < gridYDimension)
                                    {
                                        if (backgroundCharacterGrid[i + 1, j + 3].getIsOccupied())
                                        {
                                            randomDecider = j + 3;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 71;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j + 3) == false)
                                                blackListedColumns.Add(j + 3);
                                        }
                                    }
                                    else if (i == gridYDimension - 1)
                                    {

                                        randomDecider = j + 3;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 71;
                                        break;
                                    }
                                    else
                                    {
                                        if (blackListedColumns.Contains(j + 3) == false)
                                            blackListedColumns.Add(j + 3);
                                    }

                                }
                                //blue blank blue blue in a line
                                if (backgroundCharacterGrid[i, j + 1].getIsOccupied() == false && backgroundCharacterGrid[i, j + 3].getIsBlueTile() &&
                                    backgroundCharacterGrid[i, j + 2].getIsBlueTile() && backgroundCharacterGrid[i, j].getIsBlueTile())
                                {
                                    if (i + 1 < gridYDimension)
                                    {
                                        if (backgroundCharacterGrid[i + 1, j + 1].getIsOccupied())
                                        {
                                            randomDecider = j + 1;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 72;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j) == false)
                                                blackListedColumns.Add(j);
                                        }
                                    }

                                    if (i == gridYDimension - 1)
                                    {

                                        randomDecider = j + 1;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 72;
                                        break;

                                    }
                                }
                                //blue blue blank blue in a line
                                if (backgroundCharacterGrid[i, j + 2].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsBlueTile() &&
                                    backgroundCharacterGrid[i, j + 3].getIsBlueTile() && backgroundCharacterGrid[i, j].getIsBlueTile())
                                {
                                    if (i + 1 < gridYDimension)
                                    {
                                        if (backgroundCharacterGrid[i + 1, j + 2].getIsOccupied())
                                        {
                                            randomDecider = j + 2;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 73;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j + 2) == false)
                                                blackListedColumns.Add(j + 2);
                                        }
                                    }
                                    if (i == gridYDimension - 1)
                                    {

                                        randomDecider = j + 2;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 73;
                                        break;

                                    }
                                }
                            }
                            //end horizontal
                            #endregion
                            #region vertical
                            if (i < gridYDimension - 3)
                            {
                                //blank blue blue blue going down
                                if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j].getIsBlueTile() &&
                                    backgroundCharacterGrid[i + 2, j].getIsBlueTile() && backgroundCharacterGrid[i + 3, j].getIsBlueTile())
                                {

                                    randomDecider = j;
                                    i = backgroundCharacterGrid.GetLength(0);
                                    j = backgroundCharacterGrid.GetLength(1);
                                    protocol = 74;
                                    break;

                                }
                            }
                            //end vertical
                            #endregion
                            #region diagonals
                            if (i < gridYDimension - 3 && j < gridXDimension - 3)
                            {
                                //blue blue blue blank in downward diagonal
                                if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i + 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i + 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i + 3, j + 3].getIsOccupied() == false)
                                {
                                    if (i + 4 < gridYDimension)
                                    {
                                        if (backgroundCharacterGrid[i + 4, j + 3].getIsOccupied())
                                        {
                                            randomDecider = j + 3;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 78;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j + 3) == false)
                                                blackListedColumns.Add(j + 3);
                                        }
                                    }
                                    if (i + 4 == gridYDimension)
                                    {

                                        randomDecider = j + 3;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 78;
                                        break;
                                    }
                                    else
                                    {
                                        if (blackListedColumns.Contains(j + 3) == false)
                                            blackListedColumns.Add(j + 3);
                                    }


                                }
                                //blank blue blue blue downward diagonal
                                if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i + 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i + 3, j + 3].getIsBlueTile())
                                {

                                    if (backgroundCharacterGrid[i + 1, j].getIsOccupied())
                                    {
                                        randomDecider = j;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 76;
                                        break;
                                    }
                                    else
                                    {
                                        if (blackListedColumns.Contains(j) == false)
                                            blackListedColumns.Add(j);
                                    }
                                }

                                //blank blue blue blank downward diagonal
                                if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i + 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i + 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i + 3, j + 3].getIsOccupied() == false)
                                {

                                    if (backgroundCharacterGrid[i + 1, j].getIsOccupied())
                                    {
                                        lowerPriorityMove = j;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 79;
                                        break;
                                    }
                                    else
                                    {
                                        if (blackListedColumns.Contains(j) == false)
                                            blackListedColumns.Add(j);
                                    }

                                }


                                //blue blue blank blue in downward diagonal
                                if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i + 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i + 2, j + 2].getIsOccupied() == false && backgroundCharacterGrid[i + 3, j + 3].getIsBlueTile())
                                {

                                    if (backgroundCharacterGrid[i + 3, j + 2].getIsOccupied())
                                    {
                                        randomDecider = j + 2;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 5;
                                        break;
                                    }
                                    else
                                    {
                                        if (blackListedColumns.Contains(j + 2) == false)
                                            blackListedColumns.Add(j + 2);
                                    }

                                }
                            }
                            if (i > 2 && j < gridXDimension - 3)
                            {

                                //blue blue blank blue in upward diagonal
                                if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i - 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i - 2, j + 2].getIsOccupied() == false && backgroundCharacterGrid[i - 3, j + 3].getIsBlueTile())
                                {
                                    if (i - 1 > 0)
                                    {
                                        if (backgroundCharacterGrid[i - 1, j + 2].getIsOccupied())
                                        {
                                            randomDecider = j + 2;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 75;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j + 2) == false)
                                                blackListedColumns.Add(j + 2);
                                        }
                                    }
                                }
                                //blank blue blue blue in upward diagonal
                                if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i - 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i - 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i - 3, j + 3].getIsBlueTile())
                                {
                                    if (i == gridYDimension - 1)
                                    {
                                        randomDecider = j;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 56;
                                        break;
                                    }

                                    if (i + 1 < gridYDimension)
                                    {
                                        if (backgroundCharacterGrid[i + 1, j].getIsOccupied())
                                        {
                                            randomDecider = j;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 56;
                                            break;
                                        }
                                        else
                                        {
                                            if (blackListedColumns.Contains(j) == false)
                                                blackListedColumns.Add(j);
                                        }
                                    }
                                }
                                //prioritizes first one
                                //blue blue blue blank in upward diagonal
                                if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i - 1, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i - 2, j + 2].getIsBlueTile() && backgroundCharacterGrid[i - 3, j + 3].getIsOccupied() == false)
                                {
                                    if ((backgroundCharacterGrid[i - 2, j + 3].getIsOccupied()))
                                    {
                                        randomDecider = j + 3;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 53;
                                        break;
                                    }
                                    else
                                    {
                                        if (blackListedColumns.Contains(j + 3) == false)
                                            blackListedColumns.Add(j + 3);
                                    }
                                }
                            }
                            #region blockMakingWinning3InARow
                            if (j + 3 < gridXDimension)
                            {
                                //blank blue blue blank in a line
                                if (backgroundCharacterGrid[i, j].getIsOccupied() == false && backgroundCharacterGrid[i, j + 1].getIsBlueTile() &&
                                        backgroundCharacterGrid[i, j + 2].getIsBlueTile() && backgroundCharacterGrid[i, j + 3].getIsOccupied() == false)
                                {
                                    if (i == gridYDimension - 1)
                                    {
                                        if (backgroundCharacterGrid[i, j].getIsOccupied())
                                        {
                                            randomDecider = j;
                                            i = backgroundCharacterGrid.GetLength(0);
                                            j = backgroundCharacterGrid.GetLength(1);
                                            protocol = 52;
                                            break;
                                        }
                                    }

                                    if ((i + 1 < gridYDimension && j - 1 >= 0 && backgroundCharacterGrid[i + 1, j - 1].getIsOccupied()))
                                    {

                                        randomDecider = j - 1;
                                        i = backgroundCharacterGrid.GetLength(0);
                                        j = backgroundCharacterGrid.GetLength(1);
                                        protocol = 52;
                                        break;
                                    }

                                }
                            }
                            #endregion
                        }
                    }

                }
                else
                {
                    checkedBlue = false;
                }
                #endregion
                //end diagonal
                #endregion



                if (randomDecider == -1)
                {
                    if (totalTilesOnBoard < 5)
                    {
                        if (previousColumn == gridXDimension - 1)
                        {
                            randomDecider = rand.Next(gridXDimension - 2, gridXDimension);
                        }
                        else if (previousColumn == 0)
                        {
                            randomDecider = rand.Next(0, 2);
                        }
                        else
                        {
                            randomDecider = rand.Next(previousColumn - 1, previousColumn + 2);
                            while (randomDecider == previousColumn)
                            {
                                randomDecider = rand.Next(previousColumn - 1, previousColumn + 2);

                            }
                        }

                    }
                    else
                    {
                        if (lowerPriorityMove != -1)
                        {
                            randomDecider = lowerPriorityMove;
                        }
                        else
                        {
                            int testRand = rand.Next(1, 5);
                            if (isRotationEnabled && testRand == 3 && aiRotatedLastTurn == false && totalTilesOnBoard >= 4)
                            {
                                aiRotatedLastTurn = true;
                                isAiTurn = false;
                                rotateBoard();
                                shouldPlayAnimation = true;
                                if(playerTurnIndicator == 1)
                                {
                                    playerTurnIndicator = 0;
                                }
                                else
                                {
                                    playerTurnIndicator = 1;
                                }
                                return;
                            }
                            else
                            {
                                randomDecider = rand.Next(0, gridXDimension);
                                wasRandom = true;
                            }
                        }
                    }
                }

                if (isAiTurn)
                {
                    int counter = 0;
                    while ((backgroundCharacterGrid[0, randomDecider].getIsOccupied() || (blackListedColumns.Contains(randomDecider) &&
                        blackListedColumns.Count() < gridXDimension - 1)))
                    {

                        if (counter > 20)
                        {
                            while (backgroundCharacterGrid[0, randomDecider].getIsOccupied())
                            {
                                randomDecider = rand.Next(0, gridXDimension);
                            }
                            break;
                        }
                        counter++;
                        randomDecider = rand.Next(0, gridXDimension);
                        wasRandom = true;

                    }

                }

                if (isAiTurn && backgroundCharacterGrid[0, randomDecider].getIsOccupied() == false)
                {

                    if (playerTurnIndicator == 0)
                    {
                        backgroundCharacterGrid[0, randomDecider].setIsBlueTile();

                        totalTilesOnBoard += 1;
                        playerTurnIndicator = 1;
                        shouldPlayAnimation = true;
                        aiRotatedLastTurn = false;
                        if (isAiEnabled)
                            isAiTurn = false;

                    }
                    else if (playerTurnIndicator == 1)
                    {
                        backgroundCharacterGrid[0, randomDecider].setIsRedTile();
                        totalTilesOnBoard += 1;

                        shouldPlayAnimation = true;
                        aiRotatedLastTurn = false;

                        playerTurnIndicator = 0;
                        if (isAiEnabled)
                            isAiTurn = false;

                    }
                }

            }


        }

        private void checkDraws()
        {
            if (shouldPlayAnimation == false)
            {
                bool isFull = true;

                for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                {
                    if (backgroundCharacterGrid[0, j].getIsOccupied() && isFull)
                    {
                        continue;
                    }
                    else
                    {
                        isFull = false;
                        break;
                    }
                    //backgroundCharacterGrid[i, j].DrawCharacter(spriteBatch);
                }

                if (isFull)
                {
                    redWins = true;
                    blueWins = true;
                }

            }

        }

        private void resetGame()
        {

            redWins = false;
            blueWins = false;
            blackListedColumns.Clear();
            autoPlayCooldown = 0;

            totalTilesOnBoard = 0;
            for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
            {
                for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                {
                    backgroundCharacterGrid[i, j].clearColor();

                    //backgroundCharacterGrid[i, j].DrawCharacter(spriteBatch);
                }
            }
        }

        private void rotateBoard()
        {
            if (isRotationEnabled == false || shouldPlayAnimation)
            {
                return;
            }


            for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
            {
                for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                {
                    //if(backgroundCharacterGrid[i,j].getIsOccupied())
                    //{
                    //    if(j < (gridXDimension/2) )
                    //    {
                    //        if(i == (gridYDimension /2))
                    //        {
                    //            backgroundCharacterGridTemp[j, i] = backgroundCharacterGrid[i, j];
                    //        }
                    //        else if(i > (gridYDimension / 2))
                    //        {
                    //            backgroundCharacterGridTemp[j, i - ((i - (gridYDimension / 2)) * 2)] = backgroundCharacterGrid[i, j];
                    //        }
                    //        else if (i < (gridYDimension / 2))
                    //        {
                    //            backgroundCharacterGridTemp[i, j + ((gridXDimension - j) * 2)] = backgroundCharacterGrid[i, j];
                    //        }
                    //    }
                    //    else if()


                    //}
                    if (backgroundCharacterGrid[i, j].getIsOccupied())
                    {
                        if (backgroundCharacterGrid[i, j].getIsRedTile())
                        {
                            backgroundCharacterGridTemp[j, (gridXDimension - 1) - i].setIsRedTile();
                        }
                        else
                        {
                            backgroundCharacterGridTemp[j, (gridXDimension - 1) - i].setIsBlueTile();
                        }

                        backgroundCharacterGrid[i, j].clearColor();

                    }


                }
            }
            //for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
            //{
            //    for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
            //    {

            //        //backgroundCharacterGrid[i, j].clearColor();


            //    }
            //}
            for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
            {
                for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                {
                    if (backgroundCharacterGridTemp[i, j].getIsOccupied())
                    {
                        if (backgroundCharacterGridTemp[i, j].getIsRedTile())
                        {
                            backgroundCharacterGrid[i, j].setIsRedTile();
                        }
                        else
                        {
                            backgroundCharacterGrid[i, j].setIsBlueTile();
                        }
                        backgroundCharacterGridTemp[i, j].clearColor();
                    }

                }
            }
            shouldPlayAnimation = true;
            hasRotated = true;

        }

        private void arrowMovement()
        {
            arrowIndicator.setX(((selectedColumn - 1) * (screenWidth / gridXDimension)) + 20);

            arrowIndicator.moveBetween(-50, 30, 2);
        }

        private void playAnimation()
        {
            if (shouldPlayAnimation && gameClock % 10 == 0)
            {
                isCaughtUpInGravity = true;
                for (int i = backgroundCharacterGrid.GetLength(0) - 2; i >= 0; i--)
                {
                    for (int j = backgroundCharacterGrid.GetLength(1) - 1; j >= 0; j--)
                    {
                        if (backgroundCharacterGrid[i, j].getIsOccupied())
                        {
                            if (backgroundCharacterGrid[i, j].getIsBlueTile() && backgroundCharacterGrid[i + 1, j].getIsOccupied() == false)
                            {
                                backgroundCharacterGrid[i, j].clearColor();
                                backgroundCharacterGrid[i + 1, j].setIsBlueTile();
                                isCaughtUpInGravity = false;

                            }
                            else if (backgroundCharacterGrid[i, j].getIsRedTile() && backgroundCharacterGrid[i + 1, j].getIsOccupied() == false)
                            {
                                backgroundCharacterGrid[i, j].clearColor();
                                backgroundCharacterGrid[i + 1, j].setIsRedTile();
                                isCaughtUpInGravity = false;

                            }

                        }
                    }
                }

                if (isCaughtUpInGravity)
                {
                    shouldPlayAnimation = false;

                }

            }
        }

        private void movement()
        {


        }

        private void placeOnSide(int sideNum)
        {


        }

        private void drawTitleScreen()
        {


        }

        private void drawGamePlay(GameTime gameTime)
        {
            //arrowIndicator.DrawCharacterWithRotation(spriteBatch);

            for (int i = 0; i < backgroundCharacterGrid.GetLength(0); i++)
            {
                for (int j = 0; j < backgroundCharacterGrid.GetLength(1); j++)
                {
                    if (backgroundCharacterGrid[i, j].getIsRedTile())
                    {
                        backgroundCharacterGrid[i, j].DrawCharacter(spriteBatch, Color.Red);
                    }
                    else if (backgroundCharacterGrid[i, j].getIsBlueTile())
                    {
                        backgroundCharacterGrid[i, j].DrawCharacter(spriteBatch, Color.Blue);
                    }
                    else
                    {
                        backgroundCharacterGrid[i, j].DrawCharacter(spriteBatch);
                    }

                    //spriteBatch.DrawString(customfont, i + " " + j, new Vector2(backgroundCharacterGrid[i, j].GetRekt.Center.X - 5,
                    //    backgroundCharacterGrid[i, j].GetRekt.Center.Y), Color.Green);


                    //backgroundCharacterGrid[i, j].DrawCharacter(spriteBatch);
                }
            }
            arrowIndicator.DrawCharacter(spriteBatch);

            //spriteBatch.DrawString(titleAndEndFont, "It's a Draw! Press R\n     to play again", new Vector2(196, 210), Color.Green);

            //spriteBatch.DrawString(titleAndEndFont, "Blue Wins: " + blueWins, new Vector2(400, 350), Color.Blue);
            //spriteBatch.DrawString(titleAndEndFont, "Red Wins: " + redWins, new Vector2(200, 150), Color.Blue);
            if (redWins && blueWins)
            {
                spriteBatch.DrawString(titleAndEndFont, "It's a Draw! Press R\n     to play again", new Vector2(196, 210), Color.Green);

            }

            else if (redWins)
            {
                spriteBatch.DrawString(titleAndEndFont, "Red Wins! Press R\n     to play again", new Vector2(196, 210), Color.Green);
            }

            else if (blueWins)
            {
                spriteBatch.DrawString(titleAndEndFont, "Blue Wins! Press R\n     to play again", new Vector2(196, 210), Color.Green);
            }


            if (totalTilesOnBoard < 3)
            {
                //int y = 100
                int x = 370;
                spriteBatch.DrawString(customfont, "Press T to\nToggle AI", new Vector2(x, 100), Color.Blue);
                spriteBatch.DrawString(customfont, "Press J to\nToggle autoplay", new Vector2(x, 150), Color.Blue);
                spriteBatch.DrawString(customfont, "Press G to\nToggle animation", new Vector2(x, 200), Color.Blue);
                spriteBatch.DrawString(customfont, "Press P to Toggle RotationEnabled", new Vector2(x, 250), Color.Blue);
                spriteBatch.DrawString(customfont, "Press O to Rotate", new Vector2(x, 300), Color.Blue);


            }

            //spriteBatch.DrawString(customfont, "AI Enabled:\n  " + isAiEnabled, new Vector2(screenWidth - 100, 50), Color.Blue);


            //spriteBatch.DrawString(customfont, "TotalTiles: " + totalTilesOnBoard, new Vector2(120, 120), Color.Green);
            //spriteBatch.DrawString(customfont, "WasRandom: " + wasRandom, new Vector2(180, 150), Color.Green);
            //spriteBatch.DrawString(customfont, "CheckedBlue: " + checkedBlue, new Vector2(180, 190), Color.Green);
            //spriteBatch.DrawString(customfont, "Protocol: " + protocol, new Vector2(180, 60), Color.Green);
            //spriteBatch.DrawString(customfont, "IsAnimationPlaying: " + shouldPlayAnimation, new Vector2(180, 280), Color.Green);


            //spriteBatch.DrawString(customfont, "blackList: ", new Vector2(100, 30), Color.Green);
            //for (int i = 0; i < blackListedColumns.Count; i++)
            //{
            //    spriteBatch.DrawString(customfont, blackListedColumns[i].ToString(), new Vector2(100 + (20 * i), 50), Color.Green);

            //}
            //gameClock++;
        }

        private void drawOptionsScreen()
        {


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);

            // TODO: Add your drawing code here


            spriteBatch.Begin();

            switch (state)
            {

                case gameState.titleScreen:
                    drawTitleScreen();
                    break;
                case gameState.gamePlay:

                    drawGamePlay(gameTime);
                    break;

                case gameState.options:

                    drawOptionsScreen();

                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }

}
