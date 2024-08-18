using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using NAudio.Wave;

namespace WindowsFormsApp1
{
    public class CImagActor
    {
        public int X, Y;
        public Bitmap img;
        public int Speed;
        public bool isLife = true;
    }
    public class CMultiImageActor
    {
        public int X, Y;
        public List<Bitmap> imgs;
        public int iFrame;
        public int xDir = 0;
    }

    public class CActor
    {
        public int X, Y, W, H;
        public Color clr;
    }

    public class CAdvImgActor
    {
        public Bitmap wrdl;
        public Rectangle rcDst, rcSrc;
    }
    public class cv
    {
        public Rectangle rcDst;
    }
    public class cblock
    {
        public int cvalue,rvalue;
        public int bvalue ,hide;
        public int x,y;
        public Bitmap wrdl;
        public Rectangle rcDst , rcSrc;
    }
    public partial class Form1 : Form
    {
        List<CAdvImgActor> map=new List<CAdvImgActor>();
        List<CAdvImgActor> buttons = new List<CAdvImgActor>();

        cblock[,] lacts = new cblock[4, 4];
        cblock[,] bacts = new cblock[4, 4];

        List<cblock> allblocks = new List<cblock>();

        List<cv> rect = new List<cv>();

        List<string> text = new List<string> { "score", "highest score"};
        List<int> score = new List<int> { 0,0};

        List<int> xb = new List<int> {30,100,172,244};
        List<int> yb = new List<int> {204,275,351,428};

        List<int> vb = new List<int> {2,4,8,16,32,64,128,256,512,1024,2048}; 

        Bitmap off;


        Random rr = new Random();

        int flagb = -1 , swtch=1,ct=0,ctr=0,fwin=0;

        char gravity;

        public AudioFileReader backgroundMusic;
        public WaveOutEvent backgroundMusicOutput;

        public AudioFileReader soundEffect;
        public WaveOutEvent soundEffectOutput;

        public Form1()
        {
            this.Size = new Size(350, 720);
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.KeyDown += Form1_KeyDown;
            this.MouseDown += Form1_MouseDown;

           // PlaySoundEffect("effect.mp3");
        }
        int isHit(int XM, int YM)
        {
            //for (int i=0; i < LWnds.Count; i++)
            for (int i = 0;i<buttons.Count;i++)
            {
                CAdvImgActor ptrav = buttons[i];
                int X = ptrav.rcDst.X;
                int Y = ptrav.rcDst.Y;

                if (XM >= X && XM <= (X + ptrav.rcDst.Width)
                    && YM >= Y && YM <= (Y + ptrav.rcDst.Height)
                    )
                {
                    return i;
                }
            }
            return -1;
        }
        public void PlaySoundEffect(string audiofilepath)
        {
            soundEffect = new AudioFileReader(audiofilepath);
            soundEffectOutput = new WaveOutEvent();
            soundEffectOutput.Init(soundEffect);
            soundEffectOutput.Play();
            soundEffect.Volume = 0.9f;
            soundEffectOutput.Volume = 0.9f;
        }
        /*public void play()
        {
            soundEffectOutput.Play();

        }
        public void noplay()
        {
            soundEffectOutput.Pause();
        }*/
        void deleteall()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    cblock pnn = new cblock();
                    pnn.wrdl = null;
                    pnn.hide = 1;
                    pnn.bvalue = 0;
                    pnn.rcDst = new Rectangle(xb[r], yb[c], 65, 65);
                    lacts[r, c] = new cblock();
                }
            }

            allblocks = new List<cblock>();
            if (score[0] > score[1])
            {
                score[1]= score[0];
            }
            score[0]=0;
            createblock();
            DrawDubb(this.CreateGraphics());
        }
        void deletecell(cblock d)
        {
            //d.hide = 1;
            lacts[d.cvalue, d.rvalue] = null;
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            flagb = isHit(e.X, e.Y);
            if(flagb!=-1)
            {
                if(flagb==0)
                {
                    if(swtch==1)
                    {
                        swtch = 0;
                        buttons[0].wrdl = new Bitmap("mute.png");
                    }
                    else 
                    { 
                        swtch = 1;
                        buttons[0].wrdl = new Bitmap("unmute.png");
                    }
                }
                if (flagb == 1)
                {
                    for (int r = 0; r < 4; r++)
                    {
                        for (int c = 0; c < 4; c++)
                        {
                            lacts[r, c] = bacts[r, c];
                        }
                    }
                }
                if (flagb==2)
                {
                    deleteall();
                }
                flagb = -1;
            }
            DrawDubb(this.CreateGraphics());
        }

        void checksimilarity(cblock fr,cblock sc)
        {
            if(fr.bvalue==sc.bvalue) 
            { 
                deletecell(fr);
                sc.bvalue++; 
                sc.wrdl = new Bitmap(vb[sc.bvalue] + ".png"); 
                lacts[sc.cvalue, sc.rvalue] = sc;
                score[0] += vb[sc.bvalue];
                if (swtch == 1)
                {
                    PlaySoundEffect("test.mp3");
                }
            }
        }
        void createnewblock()
        {
            cblock ptrav = new cblock();
            cblock pch = new cblock();

            ptrav.wrdl = new Bitmap("2.png");
            ptrav.bvalue = 0;
            ptrav.hide = 0;
            for(; ; )
            { 
                    ptrav.rvalue = rr.Next(4);
                    ptrav.cvalue = rr.Next(4);
                    pch = lacts[ptrav.cvalue, ptrav.rvalue];
                if (pch==null)
                    {
                        break;
                    }
            }
            ptrav.rcSrc = new Rectangle(0, 0, ptrav.wrdl.Width, ptrav.wrdl.Height);
            ptrav.rcDst = new Rectangle(xb[ptrav.cvalue], yb[ptrav.rvalue], 65, 65);
            lacts[ptrav.cvalue, ptrav.rvalue] = ptrav;
            //allblocks.Add(ptrav);
            DrawDubb(this.CreateGraphics());
        }

        void checkgravity()
        {
            
            if (gravity == 'r')
            {
                for(int i = 2;i>=0;i--)
                {
                    for (int j = 0; j<4; j++)
                    {
                        cblock ptrav = lacts[i,j];
                        if (ptrav != null)
                        {
                            for (int k = i + 1; k < 4; k++)
                            {
                                cblock ptrav2 = lacts[k,j];
                                if (ptrav2==null)
                                {
                                    deletecell(lacts[k-1, j]);
                                    //ptrav = ptrav2;
                                    ptrav.hide = 0;
                                    ptrav.rvalue = j;
                                    ptrav.cvalue = k;
                                    ptrav.rcSrc = new Rectangle(0, 0, ptrav.wrdl.Width, ptrav.wrdl.Height);
                                    ptrav.rcDst = new Rectangle(xb[ptrav.cvalue], yb[ptrav.rvalue], 65, 65);
                                    lacts[k, j] = ptrav;
                                }
                                else { checksimilarity(ptrav, ptrav2); }
                                DrawDubb(this.CreateGraphics());
                            }
                        }
                    }
                }
            }
            if(gravity == 'd')
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 2; j >=0; j--)
                    {
                        cblock ptrav = lacts[i,j];
                        /*if (ptrav.hide == 0)
                        {*/
                        if (ptrav != null)
                        {
                            for (int k = j + 1; k < 4; k++)
                            {
                                cblock ptrav2 = lacts[i, k];
                                if (/*ptrav2.hide == 1 ||*/ ptrav2 == null)
                                {
                                    deletecell(lacts[i, k-1]);
                                    //ptrav = ptrav2;
                                    //ptrav = new cblock();
                                    ptrav.hide = 0;
                                    ptrav.rvalue = k;
                                    ptrav.cvalue = i;
                                    ptrav.rcSrc = new Rectangle(0, 0, ptrav.wrdl.Width, ptrav.wrdl.Height);
                                    ptrav.rcDst = new Rectangle(xb[ptrav.cvalue], yb[ptrav.rvalue], 65, 65);
                                    lacts[i, k] = ptrav;
                                }
                                else { checksimilarity(ptrav, ptrav2); }
                                DrawDubb(this.CreateGraphics());
                            }
                        }
                        //}
                    }
                }
            }
            if(gravity == 'u')
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 1; j <4; j++)
                    {
                        cblock ptrav = lacts[i,j];
                        if (ptrav!= null)
                        {
                            for (int k = j - 1; k >= 0; k--)
                            {
                                cblock ptrav2 = lacts[i,k];
                                if (ptrav2 == null)
                                {
                                    deletecell(lacts[i, k+1]);
                                    //ptrav = ptrav2;
                                    ptrav.hide = 0;
                                    ptrav.rvalue = k;
                                    ptrav.cvalue=i;
                                    ptrav.rcSrc = new Rectangle(0, 0, ptrav.wrdl.Width, ptrav.wrdl.Height);
                                    ptrav.rcDst = new Rectangle(xb[ptrav.cvalue], yb[ptrav.rvalue], 65, 65);
                                    lacts[i, k] = ptrav;
                                }
                                else { checksimilarity(ptrav, ptrav2); }
                                DrawDubb(this.CreateGraphics());
                            }
                        }
                    }
                }
            }
            if(gravity == 'l')
            {
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 0; j <4; j++)
                    {
                        cblock ptrav = lacts[i,j];
                        if (ptrav!=null)
                        {
                            for (int k = i - 1; k >= 0; k--)
                            {
                                cblock ptrav2 = lacts[k, j];
                                cblock z=new cblock();
                                if (ptrav2==null)
                                {
                                    deletecell(lacts[k+1, j]);
                                    
                                    //ptrav = ptrav2;
                                    ptrav.hide = 0;
                                    ptrav.rvalue = j;
                                    ptrav.cvalue = k;
                                    ptrav.rcSrc = new Rectangle(0, 0, ptrav.wrdl.Width, ptrav.wrdl.Height);
                                    ptrav.rcDst = new Rectangle(xb[ptrav.cvalue], yb[ptrav.rvalue], 65, 65);
                                    lacts[k,j] = ptrav;
                                    //lacts[j, i] = ptrav2;
                                }
                                else { checksimilarity(ptrav, ptrav2); }
                                DrawDubb(this.CreateGraphics());
                            }
                            
                        }
                    }
                }
            }
            //createnewblock();
        }

        void checkback()
        {
            //bacts = null;
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    //lacts[r, c] = null;
                    bacts[r, c] = null;
                    cblock ptrav = new cblock();
                    cblock ptrav2 = lacts[r, c];
                    if (ptrav2 != null)
                    {
                        ptrav.wrdl = ptrav2.wrdl;
                        ptrav.hide = ptrav2.hide;
                        ptrav.bvalue = ptrav2.bvalue;
                        ptrav.rvalue = ptrav2.rvalue;
                        ptrav.cvalue = ptrav2.cvalue;
                        ptrav.rcSrc = ptrav2.rcSrc;
                        ptrav.rcDst = ptrav2.rcDst;

                        bacts[r, c] = ptrav;
                    }
                }
            }
        }
        void checkwin()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    cblock ptrav2 = lacts[r, c];
                    if (ptrav2 != null)
                    {
                        if (ptrav2.bvalue == 10)
                        {
                            fwin = 1;
                        }
                    }   
                }
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            /*for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {

                    //bacts[r, c] = lacts[r, c];
                    bacts = new cblock[4,4];
                    cblock ptrav= bacts[r, c];
                    cblock ptrav2= lacts[r, c];
                    if (ptrav2 != null)
                    {
                        //ptrav.wrdl = ptrav2.wrdl;
                        ptrav.hide = ptrav2.hide;
                        ptrav.bvalue = ptrav2.bvalue;
                        ptrav.rvalue = ptrav2.rvalue;
                        ptrav.cvalue = ptrav2.cvalue;
                        ptrav.rcSrc = ptrav2.rcSrc;
                        ptrav.rcDst = ptrav2.rcDst;
                    }
                }
            }*/
            checkback();
            switch (e.KeyCode)
            {

                case Keys.Right:
                    gravity = 'r';
                    break;
                case Keys.Left:
                    gravity = 'l';
                    break;
                case Keys.Up:
                    gravity = 'u'; ;
                    break;
                case Keys.Down:
                    gravity = 'd'; ;
                    break;
                /*case Keys.Space:
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (lacts[i, j] != null)
                            {
                                string message = bacts[i, j].hide.ToString();
                                MessageBox.Show(message,i+j+ "Matrix Cell Information");
                            }
                        }
                    }
                    break;*/
            }
            
            checkgravity();
            createnewblock();
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            createmap();
            createblock();
        }

        void createblock()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    lacts[r, c] = null;
                    bacts[r, c] = null;
                }
            }
            cblock ptrav = new cblock();
            ptrav.wrdl = new Bitmap("2.png");
            ptrav.hide = 0;
            ptrav.bvalue = 0;
            ptrav.rvalue = rr.Next(4);
            ptrav.cvalue = rr.Next(4);
            ptrav.rcSrc = new Rectangle(0, 0, ptrav.wrdl.Width, ptrav.wrdl.Height);
            ptrav.rcDst = new Rectangle(xb[ptrav.cvalue], yb[ptrav.rvalue], 65, 65);
            //allblocks.Add(ptrav);
            lacts[ptrav.cvalue,ptrav.rvalue] = ptrav;

            bacts[ptrav.cvalue, ptrav.rvalue] = ptrav;
        }

        void createmap()
        {
            CAdvImgActor pnn = new CAdvImgActor();
            pnn.wrdl = new Bitmap("map.png");
            pnn.rcSrc = new Rectangle(0,0,pnn.wrdl.Width,pnn.wrdl.Height);
            pnn.rcDst = new Rectangle(21, 195, ClientSize.Width - 40, 150);
            map.Add(pnn);

            pnn = new CAdvImgActor();
            pnn.wrdl = new Bitmap("map.png");
            pnn.rcSrc = new Rectangle(0, 0, pnn.wrdl.Width, pnn.wrdl.Height);
            pnn.rcDst = new Rectangle(21, 344, ClientSize.Width - 40, 150);
            map.Add(pnn);

            pnn = new CAdvImgActor();
            pnn.wrdl = new Bitmap("2048.png");
            pnn.rcSrc = new Rectangle(0, 0, pnn.wrdl.Width, pnn.wrdl.Height);
            pnn.rcDst = new Rectangle(21, 50, 120, 120);
            map.Add(pnn);

            pnn = new CAdvImgActor();
            pnn.wrdl = new Bitmap("unmute.png");
            pnn.rcSrc = new Rectangle(0, 0, pnn.wrdl.Width, pnn.wrdl.Height);
            pnn.rcDst = new Rectangle(190, 120, 40, 40);
            buttons.Add(pnn);

            pnn = new CAdvImgActor();
            pnn.wrdl = new Bitmap("undo.png");
            pnn.rcSrc = new Rectangle(0, 0, pnn.wrdl.Width, pnn.wrdl.Height);
            pnn.rcDst = new Rectangle(235, 120, 40, 40);
            buttons.Add(pnn);

            pnn = new CAdvImgActor();
            pnn.wrdl = new Bitmap("reload.png");
            pnn.rcSrc = new Rectangle(0, 0, pnn.wrdl.Width, pnn.wrdl.Height);
            pnn.rcDst = new Rectangle(280, 120, 40, 40);
            buttons.Add(pnn);

            cv pnn2 = new cv();
            pnn2.rcDst = new Rectangle(180, 50, 55, 55);
            rect.Add(pnn2);


            pnn2 = new cv();
            pnn2.rcDst = new Rectangle(240, 50, 90, 55);
            rect.Add(pnn2);

            
        }
        void DrawScene(Graphics g)
        {
            g.Clear(Color.FromArgb(251, 248, 239));

            Pen Pn = new Pen(Color.FromArgb(187, 173, 160), 5);
            SolidBrush brsh = new SolidBrush(Color.FromArgb(187, 173, 160));

            for (int i = 0; i < map.Count; i++)
            {
                g.DrawImage(map[i].wrdl, map[i].rcDst, map[i].rcSrc, GraphicsUnit.Pixel);
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                g.DrawImage(buttons[i].wrdl, buttons[i].rcDst, buttons[i].rcSrc, GraphicsUnit.Pixel);
            }

            SolidBrush textBrush = new SolidBrush(Color.White);
            Font font = new Font("Arial", 10);

            for (int i = 0; i < rect.Count; i++)
            {
                SizeF textSize = g.MeasureString(text[i], font);
                SizeF scoresize = g.MeasureString(score[i].ToString(), font);

                cv ptrav = rect[i];
                g.FillRectangle(brsh, ptrav.rcDst);

                float x = rect[i].rcDst.X + (rect[i].rcDst.Width - textSize.Width) / 2;
                float y = rect[i].rcDst.Y + (rect[i].rcDst.Height - textSize.Height) / 4;

                float x2 = rect[i].rcDst.X + (rect[i].rcDst.Width - scoresize.Width) / 2;
                float y2 = rect[i].rcDst.Y + (rect[i].rcDst.Height - textSize.Height) * 3 / 4;

                g.DrawString(text[i], font, textBrush, x, y);
                g.DrawString(score[i].ToString(), font, textBrush, x2, y2);
            }

            /*for (int i = 0; i < allblocks.Count; i++)
            {
                g.DrawImage(allblocks[i].wrdl, allblocks[i].rcDst, allblocks[i].rcSrc, GraphicsUnit.Pixel);
            }*/
            if (fwin !=1)
            {
                for (int r = 0; r < 4; r++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        if (lacts[r, c] != null)
                        {
                            g.DrawImage(lacts[r, c].wrdl, lacts[r, c].rcDst, lacts[r, c].rcSrc, GraphicsUnit.Pixel);
                        }

                    }
                }
            }
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);

        }
    }
}
