/*
* PR�CTICA.............: Pr�ctica 8
* NOMBRE y APELLIDOS...: Daniel Jes�s Doblas Florido
* CURSO y GRUPO........: 2� Desarrollo de Interfaces
* T�TULO de la PR�CTICA: Uso del IDE V.Studio
* FECHA de ENTREGA.....: 8 de febrero de 2023
*/

namespace Practica8
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}