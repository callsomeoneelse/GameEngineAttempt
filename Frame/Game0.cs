using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Frame
{
    public class Game0 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spritebatch;

        public List<GameObject> objects = new List<GameObject>();
        public Map map = new Map();
        SoundEffect background;

        HUD hud = new HUD();
        Editor editor;
        public Game0() 
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Resources";

            ResX.Init(ref _graphics);
            ResX.SetVirtualResolution(800, 600);
            ResX.SetResolution(800, 600, false);

        }

        protected override void Initialize()
        {
#if DEBUG
            editor = new Editor(this);
            editor.Show();
#endif
            
            base.Initialize();

            Camera.Initialize(); //initializing camera

            Global.Initialize(this);
        }


        protected override void LoadContent()
        {
            _spritebatch = new SpriteBatch(GraphicsDevice);

#if DEBUG
            editor.LoadTextures(Content);
#endif

            map.Load(Content);

            hud.Load(Content);

            background = Content.Load<SoundEffect>("Audio//Venus");
            LoadLevel("LevelTest.lvl");

        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            UpdateObject();

            map.Update(objects);
            UpdateCamera();

#if DEBUG
            editor.Update(objects, map);
#endif

            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Red);

            ResX.BeginDraw();

            _spritebatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.GetTransformMatrix()); //implementing camera
                                                                                                                                                                                                  //causes display error if depthstencilstate set to default even though the default is none ???

#if DEBUG
            editor.Draw(_spritebatch);
#endif

            DrawObjects();

            map.DrawWalls(_spritebatch);

            _spritebatch.End();

            hud.Draw(_spritebatch);
         
            base.Draw(gameTime);
        }

        public void LoadLevel(string fileName)
        {
            Global.levelname = fileName;

            //access level data
            LevelData levelData = XML.Load("Resources//Levels//" + fileName);

            map.walls = levelData.walls;
            map.decorations = levelData.decoration;
            objects = levelData.objects;


            map.LoadMap(Content);

            background.Play(0.1f, 0, 0);

            LoadObjects();
        }

        public void LoadObjects() //initialize and load gameobjects
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Initialize();
                objects[i].Load(Content);
            }
        }

        public void UpdateObject() //update gameobject assets
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Update(objects, map);
            }
        }

        public void DrawObjects()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Draw(_spritebatch);
            }

            for (int i = 0; i < map.decorations.Count; i++)
            {
                map.decorations[i].Draw(_spritebatch);
            }
        }

        private void UpdateCamera()
        {
            if (objects.Count == 0)
            {
                return;
            }

            Camera.Update(objects[0]._position); //camera update helper method
        }
    }
}
