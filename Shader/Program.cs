using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace Shader
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Class1 cl = new Class1();
            cl.sayWord();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (shaders RayTracing = new shaders(800,600))
            {

                RayTracing.Run();  //Запускает форму
                

                
            }
        }
    }
}
