﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;

namespace camaraProfundidad
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor miKinect;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0) {
                MessageBox.Show("No se detecta ningun kinect", "Visor de Camara");
                Application.Current.Shutdown();
            }

            try
            {
                miKinect = KinectSensor.KinectSensors.FirstOrDefault();
                miKinect.DepthStream.Enable();
                miKinect.Start();
                miKinect.DepthFrameReady += miKinect_DepthFrameReady;
            }
            catch {
                MessageBox.Show("Fallo al inicializar kinect", "Visor de KInect");
                Application.Current.Shutdown();
            }
        }

        short[] datosDistancia = null;
        byte[] colorImagenDistancia = null;
        WriteableBitmap bitmapImagenDistancia = null;

        void miKinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame framesDistancia = e.OpenDepthImageFrame()) {
                if (framesDistancia == null) return;

                if (datosDistancia == null)
                    datosDistancia = new short[framesDistancia.PixelDataLength];

                if (colorImagenDistancia == null)
                    colorImagenDistancia = new byte[framesDistancia.PixelDataLength * 4];

                framesDistancia.CopyPixelDataTo(datosDistancia);

                int posColorImagenDistancia = 0;

                for (int i = 0; i < framesDistancia.PixelDataLength; i++ )
                {
                    int valorDistancia = datosDistancia[i] >> 3;

                    //if (valorDistancia == miKinect.DepthStream.UnknownDepth) {
                    if (valorDistancia < 900)
                    {
                        colorImagenDistancia[posColorImagenDistancia++] = 0; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 0; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 0; //Rojo
                    }
//                    else if (valorDistancia == miKinect.DepthStream.TooFarDepth) {
                    else if (valorDistancia >900 && valorDistancia <950  )
                    {
                        colorImagenDistancia[posColorImagenDistancia++] = 255; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 255; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 255; //Rojo
                    }
                    //else if (valorDistancia == miKinect.DepthStream.TooNearDepth)                    {
                    else if (valorDistancia > 950 && valorDistancia < 1000)
                    {
                        //verde clarita
                        colorImagenDistancia[posColorImagenDistancia++] = 55; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 210; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 49; //Rojo
                    }
                    else if (valorDistancia > 1000 && valorDistancia < 1050)
                    {
                        //verde mas oscurito
                        colorImagenDistancia[posColorImagenDistancia++] = 13; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 140; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 8 ; //Rojo
                    }
                    else if (valorDistancia > 1050 && valorDistancia < 1100)
                    {
                        //cafeshito
                        colorImagenDistancia[posColorImagenDistancia++] = 18; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 147; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 181; //Rojo
                    }
                    else if (valorDistancia > 1100 && valorDistancia < 1150)
                    {
                        //arenita cafeshita
                        colorImagenDistancia[posColorImagenDistancia++] = 73; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 189; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 251; //Rojo
                    }
                    else if (valorDistancia > 1150 && valorDistancia < 1200)
                    {
                        //ashulito clarititico
                        colorImagenDistancia[posColorImagenDistancia++] = 230; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 199; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 147; //Rojo
                    }
                    else if (valorDistancia > 1200 && valorDistancia < 1250)
                    {
                        //ashulito oscurito
                        colorImagenDistancia[posColorImagenDistancia++] = 243; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = 35; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = 8; //Rojo
                    }
                    else
                    {
                        byte byteDistancia = (byte)(255 - (valorDistancia >> 5));
                        colorImagenDistancia[posColorImagenDistancia++] = byteDistancia; //Azul
                        colorImagenDistancia[posColorImagenDistancia++] = byteDistancia; //Verde
                        colorImagenDistancia[posColorImagenDistancia++] = byteDistancia; //Rojo
                    }
                    
                    posColorImagenDistancia++;
                }

                if (bitmapImagenDistancia == null) {
                    this.bitmapImagenDistancia = new WriteableBitmap(
                        framesDistancia.Width,
                        framesDistancia.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);
                    DistanciaKinect.Source = bitmapImagenDistancia;
                }

                this.bitmapImagenDistancia.WritePixels(
                    new Int32Rect(0, 0, framesDistancia.Width, framesDistancia.Height),
                    colorImagenDistancia, //Datos de pixeles a color
                    framesDistancia.Width * 4,
                    0
                    );
            }
        }

    }
}
