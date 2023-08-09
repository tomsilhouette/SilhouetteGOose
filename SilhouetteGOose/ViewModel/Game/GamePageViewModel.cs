using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SilhouetteGOose.Model.GameState;
using SilhouetteGOose.Model.Goose;
using SilhouetteGOose.Model.Player;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace SilhouetteGOose.ViewModel.Game
{
    public partial class GamePageViewModel : ObservableObject
    {
        // States
        public SKCanvas gameCanvas;
        public GameState GameState;

        // Canvas data
        public double deviceCanvasWidth;
        public double deviceCanvasHeight;

        public float canvasW;
        public float canvasH;

        // Animation bitmaps
        private SKBitmap backgroundImageBitmap;
        private SKBitmap playerImageBitmap;
        private SKBitmap gooseLeftBitmap;
        private SKBitmap gooseRightBitmap;

        // Animation boundaries
        public SKRect PlayerRectangle;
        public SKRect CanvasRectangle;


        public float positionXmap = 0;
        public float positionYmap = 0;

        // Player
        public Player player = new();

        public float playerWidth;
        public float playerHeight;
        public float playerCenterX;
        public float playerCenterY;

        public float movementAmount = 50;

        // Goose
        public float gooseWidth;
        public float gooseHeight;

        public int gooseDirectionChangeTimer;

        // Timers
        private Timer gameTimer;
        public event EventHandler TickEvent;

        // List of enemy geese
        public List<GooseRegular> GeeseList = new();
        private List<GooseRegular> GeeseToRemove = new();

        public GamePageViewModel()
        {
            CheckScreenSize();
        }

        public GamePageViewModel(GameState gameState)
        {
            GameState = gameState;
        }
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            gameTimer = new Timer(TimeSpan.FromMilliseconds(32.0f));

            // Hook up the Elapsed event for the timer. 
            gameTimer.Elapsed += OnTimedEvent;
            gameTimer.AutoReset = true;
            gameTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            TickEvent?.Invoke(this, EventArgs.Empty);

            gooseDirectionChangeTimer++;
        }

        public void SetCanvas(SKCanvas canvas)
        {
            gameCanvas = canvas;
        }

        private void CheckScreenSize()
        {
            // Get the screen metrics
            var metrics = DeviceDisplay.MainDisplayInfo;

            // Access the screen size properties
            deviceCanvasWidth = metrics.Width;
            deviceCanvasHeight = metrics.Height;
        }

        private void CreateBitmaps()
        {
            using var backgroundStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilhouetteGOose.Resources.Images.Maps.map.png");
            backgroundImageBitmap = SKBitmap.Decode(backgroundStream);

            using var playerStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilhouetteGOose.Resources.Images.PlayerOptions.player.png");
            playerImageBitmap = SKBitmap.Decode(playerStream).Resize(new SKImageInfo(200, 200), SKFilterQuality.Low);

            using var gooseLeftStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilhouetteGOose.Resources.Images.Enemies.goose_left.png");
            gooseLeftBitmap = SKBitmap.Decode(gooseLeftStream).Resize(new SKImageInfo(100, 100), SKFilterQuality.Low);

            using var gooseRightStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SilhouetteGOose.Resources.Images.Enemies.goose_right.png");
            gooseRightBitmap = SKBitmap.Decode(gooseRightStream).Resize(new SKImageInfo(100, 100), SKFilterQuality.Low);

            // Canvas dimensions
            canvasW = backgroundImageBitmap.Width;
            canvasH = backgroundImageBitmap.Height;

            // Player dimensions
            playerWidth = playerImageBitmap.Width;
            playerHeight = playerImageBitmap.Height;

            playerCenterX = playerWidth / 2;
            playerCenterY = playerHeight / 2;

            // Goose dimensions
            gooseWidth = gooseLeftBitmap.Width;
            gooseHeight = gooseLeftBitmap.Height;
        }

        private void CreateLevelGeese()
        {
            int gooseCounter = 1;

            Random random = new();

            for (int i = 0; i < GameState.LevelNumberOfGeese; i++)
            {
                GeeseList.Add(new GooseRegular(gooseCounter, random.Next((int)canvasW - (int)gooseWidth), random.Next((int)canvasH - (int)gooseHeight), random.Next(4)));

                gooseCounter++;
            }
        }

        public void StartGame()
        {
            CheckScreenSize();
            CreateBitmaps();
            CreateLevelGeese();
            SetTimer();
        }

        public void DrawGame()
        {
            // Canvas
            var mapMat = SKMatrix.CreateScale(1f, 1f);

            gameCanvas.SetMatrix(mapMat);

            DrawAnimationsOnScreen(mapMat);

            AnimateGeese(mapMat);

            RandomiseGooseDirections();
        }

        private void DrawAnimationsOnScreen(SKMatrix mapMat)
        {
            gameCanvas.DrawBitmap(backgroundImageBitmap, new SKPoint(positionXmap, positionYmap), new SKPaint());

            var canvasRect = mapMat.Invert().MapRect(new SKRect(positionXmap, positionYmap, positionXmap + canvasW, positionYmap + canvasH));

            gameCanvas.DrawRect(canvasRect, new SKPaint()
            {
                IsStroke = true,
                StrokeWidth = 8,
                Color = SKColors.Red
            });

            CanvasRectangle = canvasRect;

            // Set player

            var playerPos = mapMat.Invert().MapPoint((float)deviceCanvasWidth / 2, (float)deviceCanvasHeight / 2);

            gameCanvas.DrawBitmap(playerImageBitmap, new SKPoint(playerPos.X - playerCenterX, playerPos.Y - playerCenterY), new SKPaint());

            var playerRect = mapMat.Invert().MapRect(new SKRect(playerPos.X - playerCenterX, playerPos.Y - playerCenterY, playerPos.X + playerWidth / 2, playerPos.Y + playerHeight / 2));

            gameCanvas.DrawRect(playerRect, new SKPaint()
            {
                IsStroke = true,
                StrokeWidth = 8,
                Color = SKColors.Red
            });

            PlayerRectangle = playerRect;
        }

        private void AnimateGeese(SKMatrix mapMat)
        {
            // DrawGeese
            foreach (GooseRegular goose in GeeseList)
            {
                if (goose.TravellingDirection == 0 || goose.TravellingDirection == 1)
                {
                    var goosePos = mapMat.Invert().MapPoint(goose.GooseXpos, goose.GooseYpos);
                    gameCanvas.DrawBitmap(gooseRightBitmap, goosePos, new SKPaint());

                    if (goose.TravellingDirection == 0)
                    {
                        goose.GooseYpos -= GameState.GooseMovementSpeed;
                    }

                    if (goose.TravellingDirection == 1)
                    {
                        goose.GooseXpos += GameState.GooseMovementSpeed;
                    }
                }
                else
                {
                    var goosePos = mapMat.Invert().MapPoint(goose.GooseXpos, goose.GooseYpos);
                    gameCanvas.DrawBitmap(gooseLeftBitmap, goosePos, new SKPaint());

                    if (goose.TravellingDirection == 2)
                    {
                        goose.GooseYpos += GameState.GooseMovementSpeed;
                    }

                    if (goose.TravellingDirection == 3)
                    {
                        goose.GooseXpos -= GameState.GooseMovementSpeed;
                    }
                }

                CheckForBoundaryGoose(goose);
            }
        }

        private void RandomiseGooseDirections()
        {
            if (gooseDirectionChangeTimer > 100)
            {
                foreach (GooseRegular goose in GeeseList)
                {
                    Random randomGooseDir = new();
                    goose.TravellingDirection = randomGooseDir.Next(4);
                }

                gooseDirectionChangeTimer = 0;
            }
        }

        private void CheckForBoundaryGoose(GooseRegular goose)
        {
            int currentDir = goose.TravellingDirection;

            if (goose.GooseXpos <= CanvasRectangle.Left)
            {
                Random randomGooseDir = new();

                goose.GooseXpos += 5;

                int newDir = randomGooseDir.Next(4);

                while (newDir == currentDir)
                {
                    newDir = randomGooseDir.Next(4);
                }

                goose.TravellingDirection = newDir;
            }
            else if (goose.GooseYpos <= CanvasRectangle.Top)
            {
                Random randomGooseDir = new();

                goose.GooseYpos += 5;

                int newDir = randomGooseDir.Next(4);

                while (newDir == currentDir)
                {
                    newDir = randomGooseDir.Next(4);
                }

                goose.TravellingDirection = newDir;
            }
            else if (goose.GooseYpos >= CanvasRectangle.Bottom - gooseHeight)
            {
                Random randomGooseDir = new();

                goose.GooseYpos -= 5;

                int newDir = randomGooseDir.Next(4);

                while (newDir == currentDir)
                {
                    newDir = randomGooseDir.Next(4);
                }

                goose.TravellingDirection = newDir;
            }
            else if (goose.GooseXpos >= CanvasRectangle.Right - gooseWidth)
            {
                Random randomGooseDir = new();

                goose.GooseXpos -= 5;

                int newDir = randomGooseDir.Next(4);

                while (newDir == currentDir)
                {
                    newDir = randomGooseDir.Next(4);
                }

                goose.TravellingDirection = newDir;
            }
        }

        [RelayCommand]
        public void MoveLeft()
        {
            if (PlayerRectangle.Left <= CanvasRectangle.Left)
            {
                Debug.WriteLine("OUT OF BOUNDS!");
            }
            else
            {
                positionXmap += movementAmount;

                foreach (GooseRegular goose in GeeseList)
                {
                    goose.GooseXpos += movementAmount;
                }
            }
        }

        [RelayCommand]
        public void MoveRight()
        {
            if (PlayerRectangle.Right >= CanvasRectangle.Right)
            {
                Debug.WriteLine("OUT OF BOUNDS!");
            }
            else
            {
                positionXmap -= movementAmount;

                foreach (GooseRegular goose in GeeseList)
                {
                    goose.GooseXpos -= movementAmount;
                }
            }
        }

        [RelayCommand]
        public void MoveUp()
        {
            if (PlayerRectangle.Top <= CanvasRectangle.Top)
            {
                Debug.WriteLine("OUT OF BOUNDS!");
            }
            else
            {
                positionYmap += movementAmount;

                foreach (GooseRegular goose in GeeseList)
                {
                    goose.GooseYpos += movementAmount;
                }
            }
        }
        [RelayCommand]
        public void MoveDown()
        {
            if (PlayerRectangle.Bottom >= CanvasRectangle.Bottom)
            {
                Debug.WriteLine("OUT OF BOUNDS!");
            }
            else
            {
                positionYmap -= movementAmount;

                foreach (GooseRegular goose in GeeseList)
                {
                    goose.GooseYpos -= movementAmount;
                }
            }
        }

        [RelayCommand]
        public void PressConfirm()
        {
            var mapMat = SKMatrix.CreateScale(1f, 1f);

            Debug.WriteLine("CCCCCCCCCCCCCCCCCCCCCCC");
            foreach (GooseRegular goose in GeeseList)
            {
                var goosePos = mapMat.Invert().MapPoint(goose.GooseXpos, goose.GooseYpos);

                if (PlayerRectangle.Contains(goosePos))
                {
                    Debug.WriteLine("QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ");

                    GeeseToRemove.Add(goose);

                }
            }
            ManageGooseRemovals();
        }

        private void ManageGooseRemovals()
        {
            foreach (GooseRegular goose in GeeseToRemove)
            {
                GeeseList.Remove(goose);
            }
        }
    }
}
