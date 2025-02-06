using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using Color = Microsoft.Xna.Framework.Color;

namespace Frame
{    

    public partial class Editor : Form
    {
        public Game0 game;
        IntPtr gameWinHandle; 

        public enum CreateMode { None, Walls, Objects, Decor };
        public CreateMode mode = CreateMode.None; 
        public bool placingItem = false; 

        Texture2D grid, pixel;
        Vector2 cameraPosition; 

        string savePath = ""; 

        enum ObjectType
        {
            Enemy, NumOfObjects,
        };

        const string objectsNamespace = "Frame."; 

        public Editor(Game0 inputGame)
        {
            InitializeComponent();                 
            game = inputGame; 
            game.IsMouseVisible = true; 
            SDL.SDL_SysWMinfo info = new SDL.SDL_SysWMinfo();
            SDL.SDL_GetWindowWMInfo(game.Window.Handle, ref info);
            gameWinHandle = info.info.win.window; 
            RECT gameWindow = new RECT();
            GetWindowRect(gameWinHandle, ref gameWindow);
            Location = new System.Drawing.Point(gameWindow.Right + 11, gameWindow.Top); 
            PopulateObjectList(); 
            mapHeight.Value = game.map.mapH;
            mapWidth.Value = game.map.mapW;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (objectTypes.SelectedIndex == -1)
                return;

            if (mode == CreateMode.Objects)
            {
                ObjectType selectedObject = (ObjectType)objectTypes.Items[objectTypes.SelectedIndex]; 
                Type type = Type.GetType(objectsNamespace + selectedObject.ToString()); 
                GameObject newObject = (GameObject)Activator.CreateInstance(type);

                if (newObject == null)
                    return;  
                newObject.Load(game.Content);
                game.objects.Add(newObject);

                placingItem = true;
                FocusGameWindow();

                SetListBox(game.objects, false);
            }
            else if (mode == CreateMode.Decor)
            {
                Decoration newDecor = new Decoration();
                newDecor.imgPath = "DirtTile";                
                newDecor.Load(game.Content, newDecor.imgPath);
                game.map.decorations.Add(newDecor);

                placingItem = true;
                SetListBox(game.map.decorations, false);
                FocusGameWindow();   
            }
        }

        public void LoadTextures(ContentManager content)
        {
            grid = TexFac.Load("128grid", content);
            pixel = TexFac.Load("pixel", content);
        }

        public void Update(List<GameObject> objects, Map map)
        {
            Vector2 mousePosition = Input.MousePositionCamera();
            Point desiredIndex = map.GetTileIDX(mousePosition); 

            if ((Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl))) &&
                    mode != CreateMode.Walls)
            { 
                if (Input.MouseLeftClicked() && mode == CreateMode.Objects)
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i].CheckCollision(new Rectangle(desiredIndex.X * map.tileSize, desiredIndex.Y * map.tileSize, 128, 128)))
                        {
                            listBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (Input.MouseLeftClicked() && mode == CreateMode.Decor)
                {
                    for (int i = 0; i < map.decorations.Count; i++)
                    {
                        if (map.decorations[i].CheckCollision(new Rectangle(desiredIndex.X * map.tileSize, desiredIndex.Y * map.tileSize, 128, 128)))
                        {
                            listBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (Input.KeyPressed(Microsoft.Xna.Framework.Input.Keys.C) && mode == CreateMode.Decor)
                    CopyDecor(); 
            }
            else if (Input.MouseLeftDown() == true && GameWindowFocused() == true)
            { 
                if (mode == CreateMode.Walls)
                {
                    #region Add Walls 
                    if (desiredIndex.X >= 0 && desiredIndex.X < map.mapW && desiredIndex.Y >= 0 && desiredIndex.Y < map.mapH)
                    {
                        Rectangle newWall = new Rectangle(desiredIndex.X * map.tileSize, desiredIndex.Y * map.tileSize, map.tileSize, map.tileSize);

                        if (map.CheckColl(newWall) == Rectangle.Empty)
                        { 
                            Rectangle oldWall = Rectangle.Empty; 
                            for (int i = 0; i < map.walls.Count; i++)
                            {
                                oldWall = map.walls[i].wall;

                                if (map.walls[i].wall.Intersects(new Rectangle(newWall.X + map.tileSize, newWall.Y, newWall.Width, newWall.Height))
                                    && map.walls[i].wall.Y == newWall.Y && map.walls[i].wall.Height == newWall.Height)
                                { 
                                    newWall = new Rectangle(oldWall.X - map.tileSize, oldWall.Y, oldWall.Width + map.tileSize, oldWall.Height);
                                    map.walls[i].wall = newWall;
                                    break;
                                }
                                else if (map.walls[i].wall.Intersects(new Rectangle(newWall.X - map.tileSize, newWall.Y, newWall.Width, newWall.Height))
                                    && map.walls[i].wall.Y == newWall.Y && map.walls[i].wall.Height == newWall.Height)
                                { 
                                    newWall = new Rectangle(oldWall.X, oldWall.Y, oldWall.Width + map.tileSize, oldWall.Height);
                                    map.walls[i].wall = newWall;
                                    break;
                                }
                                if (map.walls[i].wall.Intersects(new Rectangle(newWall.X, newWall.Y + map.tileSize, newWall.Width, newWall.Height))
                                    && map.walls[i].wall.X == newWall.X && map.walls[i].wall.Width == newWall.Width)
                                { 
                                    newWall = new Rectangle(oldWall.X, oldWall.Y - map.tileSize, oldWall.Width, oldWall.Height + map.tileSize);
                                    map.walls[i].wall = newWall;
                                    break;
                                }
                                else if (map.walls[i].wall.Intersects(new Rectangle(newWall.X, newWall.Y - map.tileSize, newWall.Width, newWall.Height))
                                    && map.walls[i].wall.X == newWall.X && map.walls[i].wall.Width == newWall.Width)
                                { 
                                    newWall = new Rectangle(oldWall.X, oldWall.Y, oldWall.Width, oldWall.Height + map.tileSize);
                                    map.walls[i].wall = newWall;
                                    break;
                                }

                                oldWall = Rectangle.Empty;
                            }

                            if (oldWall == Rectangle.Empty)
                                map.walls.Add(new Wall(newWall));

                            SetListBox(map.walls, false);
                        }
                        else
                        { 
                            for (int i = 0; i < map.walls.Count; i++)
                            {
                                if (map.walls[i].wall.Intersects(newWall))
                                {
                                    listBox.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else if (mode == CreateMode.Objects && placingItem == true)
                {
                    game.objects[game.objects.Count - 1].startPosition = game.objects[game.objects.Count - 1]._position;
                    game.objects[game.objects.Count - 1].Initialize();
                    SetListBox(game.objects, false);
                }
                else if (mode == CreateMode.Decor && placingItem == true)
                    SetListBox(game.map.decorations, false); 
                placingItem = false;
            }
            else if (Input.MouseRightDown() == true && GameWindowFocused() == true)
            {
                if (mode == CreateMode.Walls)
                {
                    #region Remove Walls 
                    Rectangle input = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 1, 1);
                    for (int i = 0; i < game.map.walls.Count; i++)
                    {
                        if (game.map.walls[i].wall.Intersects(input) == true)
                        {
                            RemoveWall(i);
                            break;
                        }
                    }
                    #endregion
                }
            }
            else if (placingItem == true)
            { 
                if (mode == CreateMode.Objects)
                    game.objects[game.objects.Count - 1]._position = new Vector2(desiredIndex.X * map.tileSize, desiredIndex.Y * map.tileSize);
                else if (mode == CreateMode.Decor)
                    game.map.decorations[game.map.decorations.Count - 1]._position = new Vector2(desiredIndex.X * map.tileSize, desiredIndex.Y * map.tileSize);
            } 
            if (paused.Checked == false && game.objects.Count > 0) 
                cameraPosition = game.objects[0]._position;
            else 
            {
                if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    cameraPosition.X += 6;
                else if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    cameraPosition.X -= 6;

                if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    cameraPosition.Y += 6;
                else if (Input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    cameraPosition.Y -= 6;

                Camera.Update(cameraPosition);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        { 
            DrawSelectedItem(spriteBatch); 
            if (drawGridCheckBox.Checked == false)
                return; 
            for (int x = 0; x < game.map.mapW; x++)
            {
                for (int y = 0; y < game.map.mapH; y++)
                    spriteBatch.Draw(grid, new Vector2(x, y) * game.map.tileSize, null, Color.Cyan, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);
            }
        }

        #region Helpers

        #region DLL Functions 
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd); 

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow(); 

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect); 
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion

        private Texture2D LoadTextureStream(GraphicsDevice graphicsDevice, string filePath)
        {  

            Texture2D file = null;
            Texture2D resultTexture;
            RenderTarget2D result = null;

            try
            {
                using (System.IO.Stream titleStream = TitleContainer.OpenStream(filePath))
                {
                    file = Texture2D.FromStream(graphicsDevice, titleStream);
                }
            }
            catch
            {
                throw new System.IO.FileLoadException("Cannot load '" + filePath + "' file!");
            }
            PresentationParameters pp = graphicsDevice.PresentationParameters; 
            result = new RenderTarget2D(graphicsDevice, file.Width, file.Height, true, pp.BackBufferFormat, pp.DepthStencilFormat);

            graphicsDevice.SetRenderTarget(result);
            graphicsDevice.Clear(Color.Black); 
            BlendState blendColor = new BlendState();
            blendColor.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;

            blendColor.AlphaDestinationBlend = Blend.Zero;
            blendColor.ColorDestinationBlend = Blend.Zero;

            blendColor.AlphaSourceBlend = Blend.SourceAlpha;
            blendColor.ColorSourceBlend = Blend.SourceAlpha;

            SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End(); 
            BlendState blendAlpha = new BlendState();
            blendAlpha.ColorWriteChannels = ColorWriteChannels.Alpha;

            blendAlpha.AlphaDestinationBlend = Blend.Zero;
            blendAlpha.ColorDestinationBlend = Blend.Zero;

            blendAlpha.AlphaSourceBlend = Blend.One;
            blendAlpha.ColorSourceBlend = Blend.One;

            spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End(); 
            graphicsDevice.SetRenderTarget(null);

            resultTexture = new Texture2D(graphicsDevice, result.Width, result.Height);
            Color[] data = new Color[result.Height * result.Width];
            Color[] textureColor = new Color[result.Height * result.Width];

            result.GetData<Color>(textureColor);

            for (int i = 0; i < result.Height; i++)
            {
                for (int j = 0; j < result.Width; j++)
                {
                    data[j + i * result.Width] = textureColor[j + i * result.Width];
                }
            }

            resultTexture.SetData(data);

            return resultTexture;
        }

        private void DrawSelectedItem(SpriteBatch spriteBatch)
        {
            if (drawSelected.Checked == false)
                return; 
            if (mode == CreateMode.Walls)
            {
                if (game.map.walls.Count == 0 || listBox.SelectedIndex >= game.map.walls.Count)
                    return;

                Wall selectedWall = game.map.walls[listBox.SelectedIndex];
                spriteBatch.Draw(pixel, new Vector2((int)selectedWall.wall.X, (int)selectedWall.wall.Y), selectedWall.wall, Color.SkyBlue, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            else if (mode == CreateMode.Objects)
            {
                if (game.objects.Count == 0 || listBox.SelectedIndex >= game.objects.Count)
                    return;

                GameObject selectedObject = game.objects[listBox.SelectedIndex];
                spriteBatch.Draw(pixel, new Vector2((int)selectedObject.BoundingBox.X, (int)selectedObject.BoundingBox.Y), selectedObject.BoundingBox, new Color(80, 80, 100, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            else if (mode == CreateMode.Decor)
            {
                if (game.map.decorations.Count == 0 || listBox.SelectedIndex >= game.map.decorations.Count)
                    return;

                Decoration selectedDecor = game.map.decorations[listBox.SelectedIndex];
                spriteBatch.Draw(pixel, new Vector2((int)selectedDecor.BoundingBox.X, (int)selectedDecor.BoundingBox.Y), selectedDecor.BoundingBox, new Color(80, 80, 100, 80), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

        private void CopyDecor()
        {
            if (listBox.SelectedIndex == -1)
                return; 
            Decoration selectedDecor = (Decoration)game.map.decorations[listBox.SelectedIndex];
            Decoration newDecor = new Decoration(selectedDecor._position, selectedDecor.imgPath, selectedDecor.layerDepth); 
            newDecor.Load(game.Content, newDecor.imgPath); 
            game.map.decorations.Add(newDecor);
            SetListBox(game.map.decorations, false);
        }

        public void RemoveWall(int index)
        {
            int bookmarkIndex = listBox.SelectedIndex;
            game.map.walls.RemoveAt(index);

            SetListBox(game.map.walls, false);
        }

        private void ResetGame()
        { 
            for (int i = 0; i < game.objects.Count; i++)
            {
                game.objects[i].Initialize();
                game.objects[i].SetDefaultPosition();
            } 
            for (int i = 0; i < game.map.decorations.Count; i++)
                game.map.decorations[i].Initialize();
        }

        public void PopulateObjectList()
        { 
            for (int i = 0; i < (int)ObjectType.NumOfObjects; i++)
                objectTypes.Items.Add((ObjectType)i);

            objectTypes.SelectedIndex = 0;
        }

        private void ResetEditorList()
        { 
            objectsRadioButton.Checked = decorRadioButton.Checked = wallsRadioButton.Checked = false;
            noneRadioButton.Checked = true;
            List<int> nothing = new List<int>();
            SetListBox(nothing, true);
            FocusGameWindow();
        }

        private void LoadLevelContent()
        { 
            for (int i = 0; i < game.map.decorations.Count; i++)
                game.map.decorations[i].Load(game.Content, game.map.decorations[i].imgPath); 
            for (int i = 0; i < game.objects.Count; i++)
            {
                game.objects[i].Initialize();
                game.objects[i].Load(game.Content);
            }
        }
        #endregion

        public void SetListBox<T>(List<T> inputList, bool highlightFirstInList)
        { 
            listBox.DataSource = null;
            listBox.DataSource = inputList;

            if (highlightFirstInList == true && inputList != null && inputList.Count > 0)
                listBox.SelectedIndex = listBox.TopIndex = 0; 
            else if (highlightFirstInList == true && inputList != null)
                listBox.SelectedIndex = listBox.TopIndex = -1; 
            else if (listBox.SelectedIndex < 0 && listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
            else
                listBox.SelectedIndex = listBox.Items.Count - 1;

            SetDisplayMember();
        }

        private void SetDisplayMember()
        { 
            if (mode == CreateMode.Walls)
                listBox.DisplayMember = "EditorWall";
            else if (mode == CreateMode.Objects)
            { 
            }
            else if (mode == CreateMode.Decor)
                listBox.DisplayMember = "Name";
        }

        public void RefreshListBox<T>(List<T> inputList)
        { 
            if (listBox.SelectedIndex - 1 >= 0)
                listBox.SelectedIndex--; 
            else
                listBox.SelectedIndex = 0; 

            placingItem = false;

            int bookmarkIndex = listBox.SelectedIndex;
            string displayMember = "";
            
                if (mode == CreateMode.Walls)
                {
                    if (bookmarkIndex == -1 && game.map.walls.Count > 0)
                        bookmarkIndex = 0;
                }
                else if (mode == CreateMode.Objects)
                {
                    if (bookmarkIndex == -1 && game.objects.Count > 0)
                        bookmarkIndex = 0;
                }
                else if (mode == CreateMode.Decor)
                {
                    if (bookmarkIndex == -1 && game.map.decorations.Count > 0)
                        bookmarkIndex = 0;
                }

            int bookmarkTopIndex = listBox.TopIndex;

            listBox.DataSource = null;
            listBox.DataSource = inputList;
            listBox.DisplayMember = displayMember;
            if (listBox.DataSource != null && inputList.Count > 0)
            {
                listBox.SelectedIndex = bookmarkIndex;
                listBox.TopIndex = bookmarkTopIndex;
            }

            SetDisplayMember();
        }

        private void wallsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (wallsRadioButton.Checked == true)
            {
                mode = CreateMode.Walls;
                SetListBox(game.map.walls, true); 
                height.Enabled = width.Enabled = true;
            }
        }

        private void objectsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (objectsRadioButton.Checked == true)
            {
                mode = CreateMode.Objects;
                SetListBox(game.objects, true); 
                objectTypes.Visible = true;
                height.Enabled = width.Enabled = false;
            }
            else
                objectTypes.Visible = false;
        }

        private void decorRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (decorRadioButton.Checked == true)
            {
                mode = CreateMode.Decor;
                SetListBox(game.map.decorations, true); 
                height.Enabled = width.Enabled = false;
                imagePath.Visible = imagePathLabel.Visible = loadImageButton.Visible = layerDepthLabel.Visible = layerDepth.Visible = decorSourceXLabel.Visible = decorSourceX.Visible =
                    decorSourceYLabel.Visible = decorSourceY.Visible = decorSourceWidthLabel.Visible = decorSourceWidth.Visible = decorSourceHeightLabel.Visible = decorSourceHeight.Visible =
                    sourceRectangleLabel.Visible = true;
            }
            else
                imagePath.Visible = imagePathLabel.Visible = loadImageButton.Visible = layerDepthLabel.Visible = layerDepth.Visible = decorSourceXLabel.Visible = decorSourceX.Visible = 
                    decorSourceYLabel.Visible = decorSourceY.Visible = decorSourceWidthLabel.Visible = decorSourceWidth.Visible = decorSourceHeightLabel.Visible = decorSourceHeight.Visible =
                    sourceRectangleLabel.Visible = false;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return; 

            int savedIndex = listBox.SelectedIndex; 

            if (mode == CreateMode.Walls)
            {
                game.map.walls.RemoveAt(listBox.SelectedIndex); 
                RefreshListBox(game.map.walls);
            }
            else if (mode == CreateMode.Objects && game.objects[listBox.SelectedIndex] is Player == false) 
            {
                game.objects.RemoveAt(listBox.SelectedIndex);
                RefreshListBox(game.objects);
            }
            else if (mode == CreateMode.Decor)
            {
                game.map.decorations.RemoveAt(listBox.SelectedIndex);
                RefreshListBox(game.map.decorations);
            } 
            placingItem = false;
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return; 
            if (mode == CreateMode.Walls)
            {
                Rectangle selectedWall = game.map.walls[listBox.SelectedIndex].wall;
                xPosition.Value = selectedWall.X;
                yPosition.Value = selectedWall.Y;
                height.Value = selectedWall.Height;
                width.Value = selectedWall.Width;
            }
            else if (mode == CreateMode.Objects)
            {
                GameObject selectedObject = game.objects[listBox.SelectedIndex];
                xPosition.Value = (int)selectedObject.startPosition.X;
                yPosition.Value = (int)selectedObject.startPosition.Y;
            }
            else if (mode == CreateMode.Decor)
            {
                Decoration selectedDecor = game.map.decorations[listBox.SelectedIndex];
                xPosition.Value = (decimal)selectedDecor._position.X;
                yPosition.Value = (decimal)selectedDecor._position.Y;
                layerDepth.Value = (decimal)selectedDecor.layerDepth;
                imagePath.Text = selectedDecor.imgPath;
                decorSourceX.Value = (decimal)selectedDecor.sourceRec.X;
                decorSourceY.Value = (decimal)selectedDecor.sourceRec.Y;
                decorSourceWidth.Value = (decimal)selectedDecor.sourceRec.Width;
                decorSourceHeight.Value = (decimal)selectedDecor.sourceRec.Height;
            }
        }

        private void xPosition_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            if (mode == CreateMode.Walls)
            {
                Rectangle selectedWall = game.map.walls[listBox.SelectedIndex].wall;
                selectedWall.X = (int)xPosition.Value;
                game.map.walls[listBox.SelectedIndex].wall = selectedWall;
            }
            else if (mode == CreateMode.Objects)
                game.objects[listBox.SelectedIndex].startPosition.X = (float)xPosition.Value;
            else if (mode == CreateMode.Decor)
                game.map.decorations[listBox.SelectedIndex]._position.X = (float)xPosition.Value;
        }

        private void yPosition_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            if (mode == CreateMode.Walls)
            {
                Rectangle selectedWall = game.map.walls[listBox.SelectedIndex].wall;
                selectedWall.Y = (int)yPosition.Value;
                game.map.walls[listBox.SelectedIndex].wall = selectedWall;
            }
            else if (mode == CreateMode.Objects)
                game.objects[listBox.SelectedIndex].startPosition.Y = (float)yPosition.Value;
            else if (mode == CreateMode.Decor)
                game.map.decorations[listBox.SelectedIndex]._position.Y = (float)yPosition.Value;
        }

        private void width_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            if (mode == CreateMode.Walls)
            {
                Rectangle selectedWall = game.map.walls[listBox.SelectedIndex].wall;
                selectedWall.Width = (int)width.Value;
                game.map.walls[listBox.SelectedIndex].wall = selectedWall;
            }
        }

        private void height_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            if (mode == CreateMode.Walls)
            {
                Rectangle selectedWall = game.map.walls[listBox.SelectedIndex].wall;
                selectedWall.Height = (int)height.Value;
                game.map.walls[listBox.SelectedIndex].wall = selectedWall;
            }
        }

        private void layerDepth_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            game.map.decorations[listBox.SelectedIndex].layerDepth = (float)layerDepth.Value;
        }

        private void loadImageButton_Click(object sender, EventArgs e) 
        {
            if (listBox.SelectedIndex == -1)
                return; 
            OpenFileDialog openFileDialog1 = new OpenFileDialog(); 
            openFileDialog1.Filter = "PNG (.png)|*.png";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false; 
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {  
                try
                { 
                    if (Directory.Exists("BackupTextures") == false)
                        Directory.CreateDirectory("BackupTextures"); 
                    File.Copy(openFileDialog1.FileName, "BackupTextures\\" + openFileDialog1.SafeFileName, true); 
                    Texture2D newImage = LoadTextureStream(game.GraphicsDevice, "BackupTextures\\" + openFileDialog1.SafeFileName); 
                    string fileName = Path.GetFileNameWithoutExtension(openFileDialog1.SafeFileName); 
                    game.map.decorations[listBox.SelectedIndex].SetImage(newImage, fileName); 
                    SetListBox(game.map.decorations, false);
                    FocusGameWindow();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error Loading Image: " + exception.Message);
                }
            }
        }

        private void menuStrip_MouseEnter(object sender, EventArgs e)
        {  
            Focus();
        }

        private void mapWidth_ValueChanged(object sender, EventArgs e)
        {
            if (mapWidth.Value > mapWidth.Maximum)
                mapWidth.Value = mapWidth.Maximum;

            game.map.mapW = (int)mapWidth.Value;
        }

        private void mapHeight_ValueChanged(object sender, EventArgs e)
        {
            if (mapHeight.Value > mapHeight.Maximum)
                mapHeight.Value = mapHeight.Maximum;

            game.map.mapH = (int)mapHeight.Value;
        }

        private void resetNPC_Click(object sender, EventArgs e)
        {
            ResetGame();
            FocusGameWindow();
        }

        private void paused_CheckedChanged(object sender, EventArgs e)
        {
            FocusGameWindow();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            game.objects.Clear();
            game.map.walls.Clear();
            game.map.decorations.Clear(); 
            game.objects.Add(new Player(Vector2.Zero)); 
            mapWidth.Value = game.map.mapW = 30;
            mapHeight.Value = game.map.mapH = 17;
            savePath = ""; 
            for (int i = 0; i < game.objects.Count; i++)
            {
                game.objects[i].Load(game.Content);
                game.objects[i].Initialize();
            } 
            ResetEditorList();
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        { 
            game.Exit();
        }

        private void decorSourceX_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            game.map.decorations[listBox.SelectedIndex].sourceRec.X = (int)decorSourceX.Value;
        }

        private void decorSourceY_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            game.map.decorations[listBox.SelectedIndex].sourceRec.Y = (int)decorSourceY.Value;
        }

        private void decorSourceWidth_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            game.map.decorations[listBox.SelectedIndex].sourceRec.Width = (int)decorSourceWidth.Value;
        }

        private void decorSourceHeight_ValueChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex == -1)
                return;

            game.map.decorations[listBox.SelectedIndex].sourceRec.Height = (int)decorSourceHeight.Value;
        }

        private void noneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (noneRadioButton.Checked == true)
            {
                mode = CreateMode.None;
                List<int> nothing = new List<int>();
                SetListBox(nothing, false);
            }
        }

        private void FocusGameWindow()
        {
            SetForegroundWindow(gameWinHandle);
        }

        private bool GameWindowFocused()
        {  
            return GetForegroundWindow() == gameWinHandle;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLevel();
        }

        private void SaveAs()
        { 
            SaveFileDialog saveFileDialog = new SaveFileDialog(); 
            savePath = ""; 
            saveFileDialog.Filter = "LEVEL (.lvl)|*.lvl";   
            try
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                { 
                    savePath = saveFileDialog.FileName; 
                    SaveLevel();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error Saving: " + exception.Message + " " + exception.InnerException);
            }
        }

        private void Save()
        { 
            if (savePath == "")
            {
                SaveAs();
                return;
            }

            try
            {
                SaveLevel();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error Saving: " + exception.Message + " " + exception.InnerException);
            }
        }

        private void SaveLevel()
        { 
            ResetGame(); 
            LevelData levelData = new LevelData()
            {
                objects = game.objects,
                walls = game.map.walls,
                decoration = game.map.decorations,
                mapW = game.map.mapW,
                mapH = game.map.mapH,
            }; 
            XML.Save(levelData, savePath);
        }

        public void OpenLevel()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog(); 
            openFileDialog1.Filter = "JORGE (.jorge)|*.jorge"; 
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LevelData levelData = XML.Load(openFileDialog1.FileName); 
                    game.objects = levelData.objects;
                    game.map.walls = levelData.walls;
                    game.map.decorations = levelData.decoration;
                    mapWidth.Value = game.map.mapW = levelData.mapW;
                    mapHeight.Value = game.map.mapH = levelData.mapH; 
                    LoadLevelContent(); 
                    if (game.objects.Count > 0)
                        Camera.LookAt(game.objects[0]._position); 
                    ResetEditorList(); 
                    savePath = ""; 
                    FocusGameWindow();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error Loading: " + exception.Message + " " + exception.InnerException);
                }
            }
        }
    }
}
