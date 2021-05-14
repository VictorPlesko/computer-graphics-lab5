using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Segmenting
{
    public partial class Form1 : Form
    {
        float wxLeft, wxRight, wyBottom, wyTop;

        public Form1()
        {
            InitializeComponent();

            Bitmap bitmap = new Bitmap(700, 400);
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen pen = new Pen(ColorTranslator.FromHtml("#87CEEB"), 3);

            string[] lines = File.ReadAllLines("input.txt");
            string[] rectCoords = lines[lines.Length - 1].Split();

            wxLeft = float.Parse(rectCoords[0]);
            wyBottom = float.Parse(rectCoords[1]);
            wxRight = wxLeft + float.Parse(rectCoords[2]);
            wyTop = wyBottom + float.Parse(rectCoords[3]);

            List<PointF> rectPoly = new List<PointF>();
            rectPoly.Add(new PointF(wxLeft, wyBottom));
            rectPoly.Add(new PointF(wxLeft, wyTop));
            rectPoly.Add(new PointF(wxRight, wyTop));
            rectPoly.Add(new PointF(wxRight, wyBottom));

            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] coordinates = lines[i].Split();

                if (coordinates.Length == 4)
                {
                    PointF p1 = new PointF(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
                    PointF p2 = new PointF(int.Parse(coordinates[2]), int.Parse(coordinates[3]));
                    graphics.DrawLine(pen, p1, p2);
                }
                else
                {
                    List<PointF> polygon = new List<PointF>();
                    for(int j = 0; j < coordinates.Length; j+=2)
                    {
                        PointF p = new PointF(int.Parse(coordinates[j]), int.Parse(coordinates[j+1]));
                        polygon.Add(p);
                    }
                    graphics.DrawPolygon(pen, polygon.ToArray());
                    graphics.FillPolygon(new SolidBrush(ColorTranslator.FromHtml("#87CEEB")), polygon.ToArray());
                }
            }

            pen.Color = ColorTranslator.FromHtml("#008000");
            graphics.DrawRectangle(pen, wxLeft, wyBottom, float.Parse(rectCoords[2]), float.Parse(rectCoords[3]));
            pictureBox1.Image = bitmap;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(700, 400);
            Graphics graphics = Graphics.FromImage(bitmap);
            Pen pen = new Pen(ColorTranslator.FromHtml("#87CEEB"), 3);

            string[] lines = File.ReadAllLines("input.txt");
            string[] rectCoords = lines[lines.Length - 1].Split();

            wxLeft = float.Parse(rectCoords[0]);
            wyBottom = float.Parse(rectCoords[1]);
            wxRight = wxLeft + float.Parse(rectCoords[2]);
            wyTop = wyBottom + float.Parse(rectCoords[3]);

            List<PointF> rectPoly = new List<PointF>();
            rectPoly.Add(new PointF(wxLeft, wyBottom));
            rectPoly.Add(new PointF(wxLeft, wyTop));
            rectPoly.Add(new PointF(wxRight, wyTop));
            rectPoly.Add(new PointF(wxRight, wyBottom));

            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] coordinates = lines[i].Split();

                if (coordinates.Length == 4)
                {
                    PointF p1 = new PointF(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
                    PointF p2 = new PointF(int.Parse(coordinates[2]), int.Parse(coordinates[3]));
                    pen.Width = 2;
                    graphics.DrawLine(pen, p1, p2);
                    CheckLine(graphics, p1, p2);
                }
                else
                {
                    List<PointF> polygon = new List<PointF>();
                    for (int j = 0; j < coordinates.Length; j += 2)
                    {
                        PointF p = new PointF(int.Parse(coordinates[j]), int.Parse(coordinates[j + 1]));
                        polygon.Add(p);
                    }
                    graphics.DrawPolygon(pen, polygon.ToArray());
                    graphics.FillPolygon(new SolidBrush(ColorTranslator.FromHtml("#87CEEB")), polygon.ToArray());

                    CheckPolygon(graphics, polygon.ToArray(), rectPoly.ToArray());
                }
            }

            pen.Color = ColorTranslator.FromHtml("#008000");
            graphics.DrawRectangle(pen, wxLeft, wyBottom, float.Parse(rectCoords[2]), float.Parse(rectCoords[3]));
            pictureBox1.Image = bitmap;
        }

        int GetPointCode(PointF point)
        {
            int i = 0;
            if (point.X < wxLeft) i++;
            else if (point.X > wxRight) i += 2;

            if (point.Y < wyBottom) i += 4;
            else if (point.Y > wyTop) i += 8;

            return i;
        }

        void CheckLine(Graphics graphics, PointF p1, PointF p2)
        {
            Pen pen = new Pen(ColorTranslator.FromHtml("#008000"), 3);

            int s;
            int code1 = GetPointCode(p1);
            int code2 = GetPointCode(p2);

            if (code1 == 0 && code2 == 0)
            {
                graphics.DrawLine(pen, p1, p2);
                return;
            }

            if ((code1 & code2) == 1)
                return;

            float dxdy = 0, dydx = 0;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            if (dx != 0)
            {
                dydx = dy / dx;
            }
            else
            {
                if (dy == 0)
                {
                    if (code1 == 0 && code2 == 0) {
                        graphics.DrawLine(pen, p1, p2);
                        return;
                    }
                    else return;
                }
            }
            if (dy != 0)
            {
                dxdy = dx / dy;
            }

            int iter = 4;
            do
            {
                if ((code1 & code2) == 1)
                    return;

                if (code1 == 0 && code2 == 0)
                {
                    graphics.DrawLine(pen, p1, p2);
                    return;
                }
                    
                if (code1 == 0)
                { 
                    s = code1; code1 = code2; code2 = s;
                    PointF p = p1; p1 = p2; p2 = p;
                }

                if ((code1 & 1) == 1)
                {     
                    p1.Y += dydx * (wxLeft - p1.X);
                    p1.X = wxLeft;
                }
                else if ((code1 & 2) == 2)
                {
                    p1.Y += dydx * (wxRight - p1.X);
                    p1.X = wxRight;
                }
                else if ((code1 & 4) == 4)
                {
                    p1.X += dxdy * (wyBottom - p1.Y);
                    p1.Y = wyBottom;
                }
                else if ((code1 & 8) == 8)
                {
                    p1.X += dxdy * (wyTop - p1.Y);
                    p1.Y = wyTop;
                }
                code1 = GetPointCode(p1);
            } while (--iter >= 0);
        }

        void CheckPolygon(Graphics graphics, PointF[] subjectPolygon, PointF[] clipPolygon)
        {
            PointF cp1, cp2, s, e;

            bool inside(PointF p)
            {
                return (cp2.X - cp1.X) * (p.Y - cp1.Y) < (cp2.Y - cp1.Y) * (p.X - cp1.X);
            }

            PointF intersection() {
                float[] dc = { cp1.X - cp2.X, cp1.Y - cp2.Y },
                    dp = { s.X - e.X, s.Y - e.Y };

                float
                    n1 = cp1.X * cp2.Y - cp1.Y * cp2.X,
                    n2 = s.X * e.Y - s.Y * e.X,
                    n3 = 1.0f / (dc[0] * dp[1] - dc[1] * dp[0]);

                PointF res = new PointF((n1 * dp[0] - n2 * dc[0]) * n3, (n1 * dp[1] - n2 * dc[1]) * n3);
                return res;
            };

            List<PointF> outputList = new List<PointF>();
            foreach (PointF point in subjectPolygon)
                outputList.Add(point);

            cp1 = clipPolygon[clipPolygon.Length - 1];
            for (int j = 0; j < clipPolygon.Length; j++)
            {
                cp2 = clipPolygon[j];
                List<PointF> inputList = outputList.ToList();
                outputList.Clear();

                if (inputList.Count > 0)
                {
                    s = inputList[inputList.Count - 1];

                    foreach (PointF item in inputList)
                    {
                        e = item;
                        if (inside(e))
                        {
                            if (!inside(s))
                            {
                                outputList.Add(intersection());
                            }
                            outputList.Add(e);
                        }
                        else if (inside(s))
                        {
                            outputList.Add(intersection());
                        }
                        s = e;
                    }
                }
                cp1 = cp2;
            }

            if (outputList.Count > 0)
            {
                Pen pen = new Pen(ColorTranslator.FromHtml("#008000"), 3);
                graphics.DrawPolygon(pen, outputList.ToArray());
                graphics.FillPolygon(new SolidBrush(ColorTranslator.FromHtml("#008000")), outputList.ToArray());
            }
        }
    }
}
