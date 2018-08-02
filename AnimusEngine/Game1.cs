﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;

namespace AnimusEngine.Desktop
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        TiledMap _map;
        TiledMapRenderer _renderer;
        public uint counter = 0;
        Map map = new Map();
        public List<GameObject> _objects = new List<GameObject>();

        //cleanup lists
        public List<GameObject> _killObjects = new List<GameObject>();
        public List<Vector2> _cameraBoundsMin = new List<Vector2>();
        public List<Vector2> _cameraBoundsMax = new List<Vector2>();
        public List<Door> _killDoors = new List<Door>();
        public List<Wall> _killWalls = new List<Wall>();


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Resolution.Init(ref _graphics);
            Resolution.SetVirtualResolution(400, 240);
            Resolution.SetResolution(800, 480, false);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        }

        protected override void Initialize()
        {
            Camera.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LevelLoader("Level_00_00");
        }

        protected override void UnloadContent()
        {
            LevelCleaner();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            UpdateCamera();
            UpdateObjects(gameTime);
            base.Update(gameTime);
            counter++;
            Console.WriteLine(counter);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Resolution.BeginDraw();
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, 
                                 SamplerState.PointClamp, null, null, null, Camera.GetTransformMatrix());

            _renderer.Draw(Camera.GetTransformMatrix());
            map.DrawWalls(_spriteBatch);
            DrawObjects();

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// Things to load in each room/level:
        /// room name, object list, camerabounds
        /// possibly screen direction / type of transition
        /// </summary>
        public void LevelLoader(string levelName)
        {
            map.Load(Content);
             _map = Content.Load<TiledMap>(levelName);
            _renderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var tileLayer in _map.TileLayers)
            {
                for (var x = 0; x < tileLayer.Width; x++)
                {
                    for (var y = 0; y < tileLayer.Height; y++)
                    {
                        var tile = tileLayer.GetTile((ushort)x, (ushort)y);

                        if (tile.GlobalIdentifier == 16)
                        {
                            var tileWidth = _map.TileWidth;
                            var tileHeight = _map.TileHeight;
                            map.walls.Add(new Wall(new Rectangle(x*tileWidth, y*tileHeight, tileWidth, tileHeight)));
                        }
                        if (tile.GlobalIdentifier == 15)
                        {
                            var tileWidth = _map.TileWidth;
                            var tileHeight = _map.TileHeight;
                            map.doors.Add(new Door(new Rectangle(x*tileWidth, y * tileHeight, tileWidth, tileHeight)));
                        }
                    }
                }
            }
            // WHYYYYYY
            foreach (var objectLayer in _map.ObjectLayers)
            {
                var objectList = objectLayer.Properties.TryGetValue("name", out string name);
                Console.WriteLine("name is: " + objectList);
            }
            Camera.cameraBoundsMaxX = 400;
            Camera.cameraBoundsMinX = 200;
            Camera.cameraBoundsMaxY = 120;
            Camera.cameraBoundsMinY = 120;

            _objects.Add(new Player(new Vector2(300, 100)));

            LoadObjects();
        }

        public void LevelCleaner()
        {
            foreach (var objects in _objects)
            {
                _killObjects.Add(objects);
            }
            foreach (var doors in map.doors)
            {
                _killDoors.Add(doors);
            }
            foreach (var walls in map.walls)
            {
                _killWalls.Add(walls);
            }
            //detroy old objects --- there must be a smarter way to do this....
            foreach (GameObject o in _killObjects)
            {
                _objects.Remove(o);
            }
            foreach (Door d in _killDoors)
            {
                map.doors.Remove(d);
            }
            foreach (Wall w in _killWalls)
            {
                map.walls.Remove(w);
            }
        }

        public void LoadObjects()
        {
            for (int i=0; i<_objects.Count; i++)
            {
                _objects[i].Initialize();
                _objects[i].Load(Content);
            }
        }

        public void UpdateObjects(GameTime gameTime)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].Update(_objects, map, gameTime);
            }
        }

        public void DrawObjects()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].Draw(_spriteBatch);
            }
        }

        public void UpdateCamera()
        {
            if (_objects.Count == 0) { return; }
            Camera.Update(_objects[0].position + new Vector2(16,0));
        }

        public static float Clamp(float value, float min, float max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }
    }
}
