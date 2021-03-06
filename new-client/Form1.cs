using System.Net.Sockets;
using System.Threading;

namespace new_client
{
    public partial class Form1 : Form
    {
        bool left = false;
        bool right = false;
        string dir = "1";

        public Form1()
        {
            InitializeComponent();
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    left = true;
                    dir = "2";
                    break;
                case Keys.D:
                case Keys.Right:
                    right = true;
                    dir = "3";
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                left = false;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                right = false;

            if(right!=left)
                dir = left ? "2" : "3";
            else dir = "1";
        }

        bool Connect(string server)
        {
            Int32 port = 15070;
            try
            {
                client = new TcpClient(server, port);
                return true;
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Size = new Size(800, 900);
            panel1.Size = new Size(200, 900);
            panel1.Location = new Point(600, 0);
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Size = new Size(200, 200);
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.Size = new Size(200, 200);
            label1.Size = new Size(100, 30);
            label1.Location = new Point(5, 100);
            label1.Font = new Font("Segoe UI", 18);
            textBoxServerIP.Size = new Size(170, 45);
            textBoxServerIP.Location = new Point(5, 144);
            textBoxServerIP.Font = new Font("Segoe UI", 18);
            btnConnect.Size = new Size(150, 50);
            btnConnect.Location = new Point(15, 220);
            label2.Size = new Size(200, 50);
            label2.Location = new Point(5, 300);
            label2.Font = new Font("Segoe UI", 24);
            player_heart.Size = new Size(155, 40);
            player_heart.Location = new Point(5, 400);
            player_heart.Font = new Font("Segoe UI", 20);

            richTextBox1.Size = new Size(250, 70);
            richTextBox1.Location = new Point(10,10);
            richTextBox1.Visible=false;

            DoubleBuffered = true;
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }
        }

        private void TalkToServer()
        {
            while (true)
            {
                SendDirection();
                ReceiveEnvironment();
                Thread.Sleep(10);
            }
            
        }

        private void SendDirection()
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(dir);
            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);
        }


        private void ReceiveEnvironment()
        {
            Byte[] buffer = new Byte[1024];
            String responseData = String.Empty;
            NetworkStream stream = client.GetStream();

            Int32 bytes = stream.Read(buffer, 0, buffer.Length);

            responseData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytes);

            string[] chunks = responseData.Split('\n');

            string myName = chunks[0];
            string[] players = chunks[1].Split('|');
            string[] blocks = chunks[2].Split('|');
            int time_elpsed = Convert.ToInt32(chunks[3]);
            start_count(chunks[4].Split(' '));
            
            if(chunks.Length > 5)
                Invoke(() => { label2.Text = chunks[5]; });
            else
                Invoke(() => { label2.Text = (time_elpsed / 1000).ToString() + "." + (time_elpsed % 1000).ToString() + " s"; });

            UpdatePlayerBlock(players, myName);
            UpdatePlatformBlock(blocks);
        }

        private void start_count(string[] statement)
        {
            if (statement[0] == "Game")
            {
                if (Int32.Parse(statement[statement.Length - 1]) < 10)
                {
                    Invoke(() =>
                    {
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = false;
                        pictureBox1.Location = new Point(200, 250);
                    });
                }
                else
                {
                    Invoke(() =>
                    {
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = true;
                        pictureBox1.Location = new Point(100, 250);
                        pictureBox2.Location = new Point(300, 250);
                    });
                }

                var temp = Int32.Parse(statement[statement.Length - 1]);
                Invoke(() =>
                {
                    switch (temp)
                    {
                        case 1:
                            pictureBox1.BackColor = Color.Red;
                            pictureBox1.Image = Properties.Resources._1;
                            break;
                        case 2:
                            pictureBox1.BackColor = Color.IndianRed;
                            pictureBox1.Image = Properties.Resources._2;
                            break;
                        case 3:
                            pictureBox1.BackColor = Color.PaleVioletRed;
                            pictureBox1.Image = Properties.Resources._3;
                            break;
                        case 4:
                            pictureBox1.Image = Properties.Resources._4;
                            break;
                        case 5:
                            pictureBox1.Image = Properties.Resources._5;
                            break;
                        case 6:
                            pictureBox1.Image = Properties.Resources._6;
                            break;
                        case 7:
                            pictureBox1.Image = Properties.Resources._7;
                            break;
                        case 8:
                            pictureBox1.Image = Properties.Resources._8;
                            break;
                        case 9:
                            pictureBox1.Image = Properties.Resources._9;
                            break;
                        case 10:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._0;
                            break;
                        case 11:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._1;
                            break;
                        case 12:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._2;
                            break;
                        case 13:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._3;
                            break;
                        case 14:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._4;
                            break;
                        case 15:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._5;
                            break;
                        case 16:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._6;
                            break;
                        case 17:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._7;
                            break;
                        case 18:
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._8;
                            break;
                        case 19:
                            pictureBox1.BackColor = Color.Transparent;
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._9;
                            break;
                    }
                });
            }
            else
            {
                Invoke(() =>
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                });
            }
        }

        //List<PlayerBlockControl> pbs = new List<PlayerBlockControl>();
        SolidBrush redBrush = new SolidBrush(Color.Orange);
        SolidBrush blueBrush = new SolidBrush(Color.MediumTurquoise);

        void UpdatePlayerBlock(string[] players, string myName)
        {
            CreateGraphics().DrawImage(Properties.Resources.background, new Rectangle(0, 0, 785, 862));
            //List<PlayerBlockControl> new_pbs = new List<PlayerBlockControl>();

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == String.Empty) continue;

                string[] playerData = players[i].Split(',');
                PlayerBlock pb = new PlayerBlock(playerData);

                //var player = pbs.Find(x => x.name == pb.name);
                //if (player == null)
                //    player = new PlayerBlockControl(CreateGraphics(), pb.name);

                //new_pbs.Add(player);

                if (pb.name == myName)
                {
                    CreateGraphics().FillRectangle(blueBrush, new Rectangle(pb.x, pb.y, pb.h, pb.w));
                    Invoke(() => player_heart.Text = "Heart : " + pb.heart.ToString());
                }
                else
                    CreateGraphics().FillRectangle(redBrush, new Rectangle(pb.x, pb.y, pb.h, pb.w));


                    // player number
                    //Graphics graphic = temp.pictureBox.CreateGraphics();
                    //Font drawFont = new Font("Serif", 16);
                    //SolidBrush drawBrush = new SolidBrush(Color.Black);
                    //float x = 1.0F;
                    //float y = 25.0F;
                    //StringFormat drawFormat = new StringFormat();
                    //drawFormat.LineAlignment = StringAlignment.Center;

                    //Invoke(() => graphic.DrawString(pb.name, drawFont, drawBrush, x, y, drawFormat));
            }

            //foreach (PlayerBlockControl pb in pbs)
            //{
            //    if (new_pbs.Find(x => x.name == pb.name) == null)
            //        pb.graphic.Dispose();
            //}

            //pbs = new_pbs;
        }

        List<PictureBox> pfs = new List<PictureBox>();
        void UpdatePlatformBlock(string[] blocks)
        {
            if (blocks.Length == 0 || blocks[0] == String.Empty)
            {
                foreach (PictureBox pf in pfs)
                {
                    Invoke(() =>
                    {
                        Controls.Remove(pf);
                        pf.Dispose();
                    });
                }
                pfs.Clear();
                return;
            }

            List<PictureBox> new_pfs = new List<PictureBox>();

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == String.Empty) continue;

                string[] blockData = blocks[i].Split(',');
                FlatformBlock pf = new FlatformBlock(blockData);

                var temp = pfs.Find(x => x.Name == pf.name);
                if (temp != null)
                {
                    new_pfs.Add(temp);
                    Invoke(() =>
                    {
                        temp.Location = new Point(pf.x, pf.y);
                    });
                }
                else
                {
                    var picture = new PictureBox();
                    new_pfs.Add(picture);

                    Invoke(() => Controls.Add(picture));
                    Invoke(() =>
                    {
                        picture.Size = new Size(pf.w, pf.h);
                        picture.Location = new Point(pf.x, pf.y);
                        picture.Name = pf.name;
                        picture.Image = (pf.type == 1 ? Properties.Resources.platform : Properties.Resources.thorn);
                        picture.SizeMode = PictureBoxSizeMode.StretchImage;
                    });
                }
            }

            foreach (PictureBox pf in pfs)
            {
                if (new_pfs.Find(x => x.Name == pf.Name) == null)
                {
                    Invoke(() =>
                    {
                        Controls.Remove(pf);
                        pf.Dispose();
                    });
                }
            }

            pfs = new_pfs;
        }

        private void btnConnet_Click(object sender, EventArgs e)
        {
            if (Connect(textBoxServerIP.Text))
            {
                textBoxServerIP.Enabled = false;
                btnConnect.Visible = false;

                Thread t1 = new Thread(TalkToServer);
                t1.Start();
            }
        }
    }
}

class PlayerBlock
{
    public int x;
    public int y;
    public int w;
    public int h;
    public string name;
    public int heart;
    
    public PlayerBlock(string[] data)
    {
        x = Convert.ToInt32(data[0]);
        y = Convert.ToInt32(data[1]);
        w = Convert.ToInt32(data[2]);
        h = Convert.ToInt32(data[3]);
        name = data[4];
        heart = Convert.ToInt32(data[5]);
    }
}

class PlayerBlockControl
{
    public Graphics graphic;
    public String name;

    public PlayerBlockControl(Graphics _graphic, String _name)
    {
        graphic = _graphic;
        name = _name;
    }
}

class FlatformBlock
{
    public int x;
    public int y;
    public int w;
    public int h;
    public string name;
    public int type;

    public FlatformBlock(string[] data)
    {
        x = Convert.ToInt32(data[0]);
        y = Convert.ToInt32(data[1]);
        w = Convert.ToInt32(data[2]);
        h = Convert.ToInt32(data[3]);
        name = data[4];
        type = Convert.ToInt32(data[5]);
    }
}