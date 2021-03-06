using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Devices.Sensors;
using Microsoft.Devices;

namespace VirtualInvestigator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class VirtualInvestigatorGame : Microsoft.Xna.Framework.Game
    {
        int score = 0;
        float bonus = 30f;
        Item house;
        Vector3[] positions = new Vector3[] { 
            new Vector3(18, 15, -11),               //1
            new Vector3(-14, 15, -10),              //2
            new Vector3(20, 7, -10),                //3 bookshelfL
            new Vector3(-18, 9, -10),               //4
            new Vector3(-8, -18, -12),              //5
            new Vector3(14, 15, -10),               //6 chr2
            new Vector3(16, -16, -10),              //7
            new Vector3(8, -18, -12),               //8
            new Vector3(-13, 14, -10),              //9
            new Vector3(-20, -20, -12),             //10
            new Vector3(-18, -6, -10),              //11
            new Vector3(-18, 0, -10),               //12
            new Vector3(-12, 0, -10),               //13 sofatable
            new Vector3(0, -18, -12),               //14 longtable
            new Vector3(0, 18, -10),                //23 commode
            new Vector3(20, 0, -10),                //15 red vase
            new Vector3(0, -18, -8),                //16 Turkey
            new Vector3(0, -18, -8),                //17 Serving Dish
            new Vector3(-20, -20, 3),               //18 blackhat
            new Vector3(-5, -8, -10),               //19 book
            new Vector3(18, 7, 1),                  //21 candilabre
            new Vector3(14, 15, -10),               //22 clothespin
            new Vector3(18, -6, -10),               //25 ant
            new Vector3(0, 18, -7),                 //26 old key
            new Vector3(-18, 9, -7),                //27 grain bowl
            new Vector3(-21, -15, -11),             //28 guitar
            new Vector3(18, -18, -7)                //29 bulb
            };
        string[] models = new string[]{ 
            "tableSmall",                           // 1
            "armchair_vi",                          // 2
            "bookcaseL_vi",                         // 3
            "bookcaseM_vi",                         // 4
            "ChairI_vi",                            // 5
            "ChairII_vi",                           // 6
            "ChestWood_vi",                         // 7
            "ChairI_vi2",                           // 8
            "Footrest_vi",                          // 9
            "HighShelf_vi",                         // 10
            "SmokingTable_vi",                      // 11
            "sofa_vi",                              // 12
            "sofatable_vi",                         // 13
            "TableLong_vi",                         // 14
            "Commode_vi",                           // 23
            "Vase01",                               // 15
            "Serving_vi",                           // 16
            "Turkey_vi",                            // 17
            "blackhat",                             // 18
            "Book1_vi",                             // 19
            "Candilabre_vi",                        // 21
            "clothespin",                           // 22
            "ant",                                  // 25
            "oldKey",                               // 26
            "Grain_vi",                             // 27
            "guitar",                               // 28
            "lightBulb"                             // 29
                
        };
        string[] modelNames = new string[] { 
            "Small Table",                          // 1
            "Arm Chair",                            // 2
            "Bookcase Large",                       // 3
            "Bookcase Med",                         // 4
            "Chair I",                              // 5
            "Chair II",                             // 6
            "Chest Wood",                           // 7
            "Chair I",                              // 8
            "Foot Rest",                            // 9
            "High Shelf",                           // 10
            "Smoking Table",                        // 11
            "Sofa",                                 // 12
            "Sofa Table",                           // 13
            "Long Table",                           // 14
            "Commode",                              // 23
            "Red Vase",                             // 15
            "Serving Plate",                        // 16
            "Turkey",                               // 17
            "Black Hat",                            // 18
            "Book",                                 // 19
            "Candilabre",                           // 21
            "Clothespin",                           // 22
            "Ant",                                  // 24
            "Old Key",                              // 26
            "Bowl",                                 // 27
            "Guitar",                               // 28
            "Light Bulb"                            // 29
        };
        List<string> findList = new List<string> { 
        };

        string modelPointAt = "";
        string touched = "";
        float DisplayTime = 0f;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        
        //sound stuff -Kevin
        SoundEffect hitSound;
        SoundEffect missSound;
        SoundEffect music;
        SoundEffectInstance instance;
        //when creating a new item:
        //
        #region 2D
        Texture2D crossHair;
        #endregion

        String intersecting = "false";

        Motion motion;
        Matrix attitude;

        Camera camera;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        int bulletCntDwn = 90;

        String yaw = "", pitch = "", roll = "", strCamPos = "";

        String screenData;

        //Accelerometer accelerometer;
        //Compass compass;

        //Basic effect object
        BasicEffect basicEffect;

        // vertex data with position and color
        VertexPositionColor[] pointList;

        // vertex buffer to hold the vertex data
        VertexBuffer vertexBuffer;
        //Vector3 position;

        GameTime displayFoundMsgTime;

        //Models
        Item[] items;
        List<Bullet> bullets;

        // the left and right hit region on the screen for rotating the axes
        Rectangle recLeft;
        Rectangle recRight;
        Rectangle recUp;
        Rectangle recDown;

        double prevX = 0, prevY = 0, prevZ = 0;
        double prevHeading = 0, heading = 0;

        Vector3 angle;

        Vector2 gestureDelta;

        float rotation = 45;

        public VirtualInvestigatorGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default forr Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        public void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {

          double myX =  e.X ;

          if (Math.Abs(myX - prevX) > 0.035)
            {
                camera.View *= Matrix.CreateRotationX((float)(Math.PI * (myX - prevX)));

                prevX = myX;
            }
         /* else if (myX - prevX < 0.5)
          {
              camera.View *= Matrix.CreateRotationX((float)(Math.PI * (prevX - myX)));

              prevX = myX;
          }*/
         
            
            //if (Math.Abs(e.Y - prevY) > 0.04)
            //{
            //    camera.View *= Matrix.CreateRotationY((float)(Math.PI * (e.Y - prevY)));
            //
            //    prevY = e.Y;
            //}
            
        }

        public void CompassReadingChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            heading = e.SensorReading.TrueHeading;

            if (Math.Abs(heading - prevHeading) > 8 )
            {
                camera.View *= Matrix.CreateRotationY((float)(Math.PI * (heading - prevHeading) / 180 ));

                prevHeading = heading;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = Matrix.Identity;

            //pointList = new VertexPositionColor[6];
            //pointList[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
            //pointList[1] = new VertexPositionColor(new Vector3(50, 0, 0), Color.Red);
            //pointList[2] = new VertexPositionColor(new Vector3(0, 0, 0), Color.White);
            //pointList[3] = new VertexPositionColor(new Vector3(0, 50, 0), Color.White);
            //pointList[4] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue);
            //pointList[5] = new VertexPositionColor(new Vector3(0, 0, 50), Color.Blue);
            //
            //vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, 6, BufferUsage.None);
            //vertexBuffer.SetData<VertexPositionColor>(pointList);

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            crossHair = Content.Load<Texture2D>("magGlass");

            //sound load stuff
            hitSound = Content.Load<SoundEffect>("shimmer_1");
            missSound = Content.Load<SoundEffect>("click");
            music = Content.Load<SoundEffect>("Bushwick");
            instance = music.CreateInstance();
            instance.IsLooped = true;
            music.Play();


            //randomize find list
            Random rand = new Random();
            int index;

            while (findList.Count < 3)
            {
                index = rand.Next(0, 11);
                if(!findList.Contains(modelNames[modelNames.Length - index]))
                {
                    findList.Add(modelNames[modelNames.Length - index]);
                }
            }
            //accelerometer = new Accelerometer();
            //accelerometer.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(AccelerometerReadingChanged);
            //
            //compass = new Compass();
            //compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(400);
            //compass.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<CompassReading>>(CompassReadingChanged);
            //
            //try
            //{
            //    compass.Start();
            //    accelerometer.Start();
            //}
            //catch (Exception e)
            //{
            //    Exit();
            //}

            if (!Motion.IsSupported)
            {
                //MessageBox.Show("the Motion API is not supported on this device.");
                return;
            }

            // If the Motion object is null, initialize it and add a CurrentValueChanged
            // event handler.
            if (motion == null)
            {
                motion = new Motion();
                motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
                motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);

            }

            // Try to start the Motion API.
            try
            {
                motion.Start();
            }
            catch (Exception)
            {
                //MessageBox.Show("unable to start the Motion API.");
                Exit();
            }

            InitializeCamera();
            InitializeRocket();

            angle = new Vector3();

            // Enable the FreeDrag gesture 
            TouchPanel.EnabledGestures = GestureType.FreeDrag; 
            
            // Define the camera position and the target position 
            camera.position = new Vector3(0, 0, 0);
            Vector3 target = new Vector3(0, 0, -10);
            Matrix original = camera.View;
            camera.View = Matrix.CreateLookAt(camera.position, target, Vector3.Backward);
            // Create the camera View matrix and Projection matrix 
            if (camera.View.Forward.X.Equals(System.Single.NaN) || camera.View.Forward.Y.Equals(System.Single.NaN))
            {
                camera.View = Matrix.CreateLookAt(camera.position, target, Vector3.Up);
            }
            else
            {
                camera.View = Matrix.CreateLookAt(camera.position, target, Vector3.Backward);
            }
            camera.Projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 1000.0f); 
            
            base.Initialize();
        }

        public void InitializeCamera()
        {
            camera = new Camera(this, GraphicsDevice)
                {
                    originalPosition = new Vector3(0, 0, 0)
                };
            camera.Initialize();
        }

        public void InitializeRocket()
        {
            //initialize house
            house = new Item(this, "house2")
            {
                Position = new Vector3(0, 0, -30),
                cam = camera
            };
            house.Initialize();

            items = new Item[models.Length];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new Item(this, models[i])
                {
                    Position = positions[i],
                    cam = camera,
                    displayName = modelNames[i]
                };
                items[i].Initialize();
            }

            //
            //rockets[1] = new Rocket(this)
            //{
            //    Position = new Vector3(-100, 0, 0),
            //    cam = camera
            //};
            //rockets[1].Initialize();
            //
            //rockets[2] = new Rocket(this)
            //{
            //    Position = new Vector3(100, 0, 0),
            //    cam = camera
            //};
            //rockets[2].Initialize();
            //
            //rockets[3] = new Rocket(this)
            //{
            //    Position = new Vector3(0, 100, 0),
            //    cam = camera
            //};
            //rockets[3].Initialize();
            //
            //rockets[4] = new Rocket(this)
            //{
            //    Position = new Vector3(0, -100, 0),
            //    cam = camera
            //};
            //rockets[4].Initialize();

            bullets = new List<Bullet>();
        }

        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            // This event arrives on a background thread. Use BeginInvoke
            // to call a method on the UI thread.
            //Dispatcher.BeginInvoke(() => CurrentValueChanged(e.SensorReading));
            CurrentValueChanged(e.SensorReading);
        }



        private void CurrentValueChanged(MotionReading reading)
        {

            if (screenData == null)
            {
                screenData = "Yaw : " +reading.Attitude.Yaw.ToString() +"\nPitch : " +reading.Attitude.Pitch.ToString() +"\nRoll : " +reading.Attitude.Roll.ToString();
            }


            // Get the RotationMatrix from the MotionReading.
            // Rotate it 90 degrees around the X axis to put it in xna coordinate system.

            if (reading.Gravity.Z < 0)
                attitude = Matrix.CreateFromYawPitchRoll(-reading.Attitude.Pitch, reading.Attitude.Roll, -reading.Attitude.Yaw); // reading.Attitude.RotationMatrix; //Matrix.CreateRotationZ(MathHelper.PiOver2) * reading.Attitude.RotationMatrix;
            else
            {
                float difference = -MathHelper.Pi - reading.Attitude.Roll;
                float difference2 = MathHelper.Pi - reading.Attitude.Pitch;
                float difference3 = MathHelper.Pi - reading.Attitude.Yaw;
                attitude = Matrix.CreateFromYawPitchRoll(difference2, difference, difference3); // reading.Attitude.RotationMatrix; //Matrix.CreateRotationZ(MathHelper.PiOver2) * reading.Attitude.RotationMatrix;
            }


            camera.View = attitude;

            yaw = reading.Attitude.Yaw.ToString();
            pitch = reading.Attitude.Pitch.ToString();
            roll = reading.Attitude.Roll.ToString();

            // Loop through the points in the list
            //for (int i = 0; i < points.Count; i++)
            //{
            //    // Create a World matrix for the point.
            //    Matrix world = Matrix.CreateWorld(points[i], new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            //
            //    // Use Viewport.Project to project the point from 3D space into screen coordinates.
            //    Vector3 projected = viewport.Project(Vector3.Zero, projection, view, world * attitude);
            //
            //
            //    if (projected.Z > 1 || projected.Z < 0)
            //    {
            //        // If the point is outside of this range, it is behind the camera.
            //        // So hide the TextBlock for this point.
            //        textBlocks[i].Visibility = Visibility.Collapsed;
            //    }
            //    else
            //    {
            //        // Otherwise, show the TextBlock
            //        textBlocks[i].Visibility = Visibility.Visible;
            //
            //        // Create a TranslateTransform to position the TextBlock.
            //        // Offset by half of the TextBlock's RenderSize to center it on the point.
            //        TranslateTransform tt = new TranslateTransform();
            //        tt.X = projected.X - (textBlocks[i].RenderSize.Width / 2);
            //        tt.Y = projected.Y - (textBlocks[i].RenderSize.Height / 2);
            //        textBlocks[i].RenderTransform = tt;
            //    }
            //}
        }




        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //rocket = Content.Load<Model>("missile_2");
            // TODO: use this.Content to load your game content here
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
            if (DisplayTime > 0)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                DisplayTime -= elapsed;
            }

            if (bonus > 0)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                bonus -= elapsed;
            }

            //elapsedTime += gameTime.ElapsedGameTime;
            //
            //if (elapsedTime > TimeSpan.FromSeconds(1))
            //{
            //    elapsedTime -= TimeSpan.FromSeconds(1);
            //    frameRate = frameCounter;
            //    frameCounter = 0;
            //}
            
            camera.Update(gameTime);

            //cast a ray in front of the camera
            Matrix lookAt = Matrix.Invert(camera.View);
            Vector3 direction = Vector3.Transform(new Vector3(0, 0, -20), lookAt);
            direction.Normalize();
            Ray ray = new Ray(camera.position, direction);




            //bulletCntDwn -= 1;
            //
            //if (bulletCntDwn == 0)
            //{
            //    bulletCntDwn = 90;
            //
            //    bullets.Add(new Bullet(this, camera){ Position = new Vector3(0, 0, -3) });
            //    bullets.Last().Initialize();
            //}

            //move bullets
            foreach (Bullet bullet in bullets)
            {
                bullet.Update(gameTime);
            }

            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
           //     this.Exit();

            intersecting = "false";
            modelPointAt = "";

            house.Update(gameTime);

            foreach(Item item in items) 
            {
                item.Update(gameTime);
                Nullable<float> boxDistance = item.box.Intersects(ray);
                if (boxDistance != null)
                {
                    intersecting = "true";
                    modelPointAt = item.displayName;
                }
            }

            strCamPos = camera.position.ToString();
            
            // TODO: Add your update logic here
            TouchCollection touches = TouchPanel.GetState();
            if (touches.Count > 0 && touches[0].State == TouchLocationState.Pressed)
            {
                if (modelPointAt != "")
                {
                    touched = modelPointAt;
                    if (findList.Contains(touched))
                    {
                        score += 25000;
                        if (bonus > 0)
                            score += (int)(bonus * 75000);
                        DisplayTime = 1f;
                        findList.Remove(touched);
                        hitSound.Play();
                    }
                    else
                    {
                        missSound.Play();
                    }
                }
                else
                {
                    missSound.Play();
                }
            }


            base.Update(gameTime);
        }

        public void DrawModel(Model model) 
        {
            Matrix[] transforms = new Matrix[model.Bones.Count]; 
            model.CopyAbsoluteBoneTransformsTo(transforms); 
            
            // Draw the model. A model can have multiple meshes. 
            foreach (ModelMesh mesh in model.Meshes) 
            {
                foreach (BasicEffect effect in mesh.Effects) 
                {
                    //effect.EnableDefaultLighting(); 
                    effect.World = transforms[mesh.ParentBone.Index]; 
                    effect.View = camera.View; 
                    effect.Projection = camera.Projection; 
                } 
                mesh.Draw(); 
            } 
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //frameCounter++;

            string scoreTxt = string.Format("SCORE: {0}", score);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default; 
            GraphicsDevice.BlendState = BlendState.Opaque;

            basicEffect.World = Matrix.CreateRotationY(MathHelper.ToRadians(rotation)) *
                                Matrix.CreateRotationX(MathHelper.ToRadians(50));
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;

            basicEffect.VertexColorEnabled = true;

            //foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.SetVertexBuffer(vertexBuffer, 0);
            //    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, pointList, 0, 3);
            //}

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(gameTime);
            }

            house.Draw(gameTime);

            foreach (Item rocket in items)
            {
                rocket.Draw(gameTime);
                //BoundingBoxBuffers buffers = CreateBoundingBoxBuffers(rocket.box, GraphicsDevice);
                //DrawBoundingBox(buffers, basicEffect, GraphicsDevice, camera.View, camera.Projection);


                //BoundingBox b = new BoundingBox(new Vector3(drone.Position.X - drone.limit, drone.Position.Y - drone.limit, drone.Position.Z - drone.limit),
                //                   new Vector3(drone.Position.X + drone.limit, drone.Position.Y + drone.limit, drone.Position.Z + drone.limit));
                //BoundingBoxBuffers buffers = CreateBoundingBoxBuffers(b, SharedGraphicsDeviceManager.Current.GraphicsDevice);
                //DrawBoundingBox(buffers, basicEffect, SharedGraphicsDeviceManager.Current.GraphicsDevice, camera.View, camera.Projection);
            
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, scoreTxt, new Vector2(50, 25), Color.Black);
            spriteBatch.DrawString(font, scoreTxt, new Vector2(49, 24), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(crossHair, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), null, Color.White, 0, new Vector2(crossHair.Width / 2, crossHair.Height / 2), 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            //string message = string.Format("Current Data \n Yaw: {0} \n Pitch: {1} \n Roll: {2} \n CamPos: {3}", yaw, pitch, roll, strCamPos);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, modelPointAt, new Vector2(50, 50), Color.Black);
            spriteBatch.DrawString(font, modelPointAt, new Vector2(49, 49), Color.White);
            spriteBatch.End();

            for (int i = 0; i < findList.Count; i++)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, findList[i], new Vector2(20 + i * 125, GraphicsDevice.Viewport.Height - 50), Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, findList[i], new Vector2( 20 + i * 125, GraphicsDevice.Viewport.Height - 50), Color.White, 0f, new Vector2(1,1), 1f, SpriteEffects.None, 0);
                spriteBatch.End();
            }

            if (DisplayTime > 0)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "You found " + touched, new Vector2(250, 175), Color.Black);
                spriteBatch.DrawString(font, "You found " + touched, new Vector2(249, 174), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawBoundingBox(BoundingBoxBuffers buffers, BasicEffect effect, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            graphicsDevice.SetVertexBuffer(buffers.Vertices);
            graphicsDevice.Indices = buffers.Indices;

            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                    buffers.VertexCount, 0, buffers.PrimitiveCount);
            }
        }

        private BoundingBoxBuffers CreateBoundingBoxBuffers(BoundingBox boundingBox, GraphicsDevice graphicsDevice)
        {
            BoundingBoxBuffers boundingBoxBuffers = new BoundingBoxBuffers();

            boundingBoxBuffers.PrimitiveCount = 24;
            boundingBoxBuffers.VertexCount = 48;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice,
                typeof(VertexPositionColor), boundingBoxBuffers.VertexCount,
                BufferUsage.WriteOnly);
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            const float ratio = 5.0f;

            Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0);
            Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0);
            Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio);
            Vector3[] corners = boundingBox.GetCorners();

            // Corner 1.
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] + xOffset);
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] - yOffset);
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] - zOffset);

            // Corner 2.
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - xOffset);
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - yOffset);
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - zOffset);

            // Corner 3.
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] - xOffset);
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] + yOffset);
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] - zOffset);

            // Corner 4.
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] + xOffset);
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] + yOffset);
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] - zOffset);

            // Corner 5.
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] + xOffset);
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] - yOffset);
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] + zOffset);

            // Corner 6.
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] - xOffset);
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] - yOffset);
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] + zOffset);

            // Corner 7.
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] - xOffset);
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] + yOffset);
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] + zOffset);

            // Corner 8.
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + xOffset);
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + yOffset);
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + zOffset);

            vertexBuffer.SetData(vertices.ToArray());
            boundingBoxBuffers.Vertices = vertexBuffer;

            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, boundingBoxBuffers.VertexCount,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(Enumerable.Range(0, boundingBoxBuffers.VertexCount).Select(i => (short)i).ToArray());
            boundingBoxBuffers.Indices = indexBuffer;

            return boundingBoxBuffers;
        }

        private static void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
        {
            vertices.Add(new VertexPositionColor(position, Color.White));
        }

    }
}
