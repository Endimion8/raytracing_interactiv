using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.IO;
using OpenTK.Input;


namespace Shader
{
    class shaders:GameWindow
    {



        #region data
        int BasicProgramID; //Номер дескриптора на графической карте
        int BasicVertexShader; //Адрес вершинного шейдера  
        int BasicFragmentShader; //Адрес фрагментного шейдера

        float angelX = 0.0f; // 0 to 360
        float angelY = 0.0f; //-90 to 90
        int mouseX = 0;
        int mouseY = 0;
        float dx=0, dy=0, dz=0;
        int Wx = 0, Wy = 0;
        int time;

        int attribute_vpos; //Адрес параметра позиции
        int vbo_position; //Адрес буфера вершин объекта для нашего параметра позиции
        int uniform_pos;
        int uniform_aspect;
        int uniform_sphereCenter;
        int uniform_lposition;
        Vector3 sphereCenter;
        Vector3 lposition;
        float aspect;
        Vector3 campos ;
        Vector3[] vertdata; //Массив позиций вершин
        #endregion

        #region zagruzkashader
        void loadShader(String filename, ShaderType type, int program, out int address)
        {

            address = GL.CreateShader(type);  //Создает объект шрейдера с одним из типов
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());    // Загружает исходный код в созданный шейдерный объект
            }
            GL.CompileShader(address);  // Компиляция шейдера
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }
        private void InitShaders()
        {
            
            BasicProgramID = GL.CreateProgram();  // создание объекта программы 
            loadShader("..\\..\\raytracing.vs", ShaderType.VertexShader,   BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\raytracing.fs", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);//Компановка программы

            // Проверить успех компоновки
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));

            attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vPosition");
            uniform_pos = GL.GetUniformLocation(BasicProgramID, "campos");
            uniform_aspect= GL.GetUniformLocation(BasicProgramID, "aspect");
            uniform_sphereCenter = GL.GetUniformLocation(BasicProgramID, "sphereCenter");
            uniform_lposition = GL.GetUniformLocation(BasicProgramID, "lposition");

            GL.GenBuffers(1, out vbo_position);   ///
        }
        #endregion
        
        public shaders(int width, int hight)
            : base(width, hight)
        {
            //CursorVisible = false;
            Wx = this.X; Wy = this.Y;
        }


        protected override void OnLoad(EventArgs e)   //Вызывает событие Load
        {
            base.OnLoad(e);

            InitShaders();

            vertdata = new Vector3[] {
                new Vector3(-1f, -1f, 0f),
                new Vector3( 1f, -1f, 0f),
                new Vector3( 1f,  1f, 0f),
                new Vector3(-1f,  1f, 0f) };
            /////////////////////////////////////////////////
            campos = new Vector3(0, 0, -8);
            sphereCenter = new Vector3(-1f, -1f, -1f);
            lposition = new Vector3(0.0f, 2.0f, -4.0f);
            ////////////////////////////////////////
        }

        

        protected override void OnRenderFrame(FrameEventArgs e)   //Отвечает за перересовку
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);  //Очищать буфер
            GL.Enable(EnableCap.DepthTest);    //Дальние элементы перекрываются ближними

            
            


            GL.EnableVertexAttribArray(attribute_vpos);   //Активация атрибутов вершин
            GL.DrawArrays(PrimitiveType.Quads,0, 4);      //Отображает нашу фигуру
            GL.DisableVertexAttribArray(attribute_vpos);  //Выключаем режим отрисовки




            SwapBuffers();   //Быстро скопировать содержимое заднего буфера окна в передний буфер
        }
        protected override void OnUpdateFrame(FrameEventArgs e)   //Отвечает за обновление Update
        {
            base.OnUpdateFrame(e);

            time++;
            //sphereCenter.Y = (float)Math.Sin((2 * Math.PI / 2) *time* 0.1f ) * 5;
            //sphereCenter.X = (float)Math.Sin((2 * Math.PI / 2) * time * 0.1f) * 3 -1;
            //sphereCenter.Z = (float)Math.Cos((2 * Math.PI / 2) * time * 0.1f) * 3 -1;
           
           // lposition.X = (float)Math.Cos((2 * Math.PI / 2) * time * 0.04f) * 3 - 1;
            //lposition.Z = (float)Math.Sin((2 * Math.PI / 2) * time * 0.04f) * 3 - 1; 

            aspect = (float)Width / Height;

            var mouse = OpenTK.Input.Mouse.GetCursorState();
            var state = OpenTK.Input.Keyboard.GetState();
            
            mouseX = mouse.X;
            mouseY = mouse.Y;

            int xc = Wx + Width / 2;
            int yc = Wy + Height / 2;

           //OpenTK.Input.Mouse.SetPosition(xc, yc);

            angelX = (xc - mouseX) / 5;
            angelY = (yc - mouseY) / 5;
            //Console.WriteLine(campos.Y);
            if (angelY < -89.0f) angelY = -89.0f;

           if (angelY > 89.0f) angelY = 89.0f;


           float speed = 0.5f;

           campos.X += dx*speed;
           campos.Y += dy*speed;
           campos.Z += dz*speed;
           if (campos.Y < 1 || campos.Y > 100)
               campos.Y = 0;
           if (campos.X < -50 || campos.X > 50)
               campos.X = 0;
           if (campos.Z < -50 || campos.Z > 100)
               campos.Z = -8;

           dx = dz = dy = 0; 

            if (state[Key.W])
            {
                dz = 1 * speed;
            }

            if (state[Key.S])
            {
                dz = -1 *speed;
            }

            if (state[Key.D])
            {
                dx = 1 * speed;
            }

            if (state[Key.A])
            {
                dx = -1 * speed;

            }
            
            if (state.IsKeyUp(Key.W) && state.IsKeyUp(Key.S) && state.IsKeyUp(Key.D) && state.IsKeyUp(Key.A) && mouse.IsButtonDown(MouseButton.Left))
            {
                dx= (float)Math.Sin(angelX / 180 * Math.PI) *0.5f ;
                dy = (float)Math.Tan(angelY / 180 * Math.PI) * 0.5f;
                dz = (float)Math.Tan(angelY / 180 * Math.PI) * 0.5f;
            }

            
            /*campos = new Vector3((float)Math.Sin(angelX / 180 * Math.PI) ,
                     (float)Math.Tan(angelY / 180 * Math.PI) ,
                     (float)Math.Tan(angelY / 180 * Math.PI) );*/


            if (state[Key.Escape])
               Exit();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.Uniform3(uniform_pos, campos);
            GL.Uniform3(uniform_sphereCenter, sphereCenter);
            GL.Uniform3(uniform_lposition, lposition);
            GL.Uniform1(uniform_aspect, aspect);
            
            GL.UseProgram(BasicProgramID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

     
        
    }
}
