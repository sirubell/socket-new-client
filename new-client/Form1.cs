using System.Net.Sockets;
using System.Drawing;

namespace new_client
{ 
    public partial class Form1 : Form
    {
        string dir = "1";
        public Form1()
        {
            InitializeComponent();

            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    dir = "2";
                    break;
                case Keys.D:
                case Keys.Right:
                    dir = "3";
                    break;

            }
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left || e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                dir = "1";
        }

        bool Connect(string server)
        {
            Int32 port = 12345;
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
            this.Size = new Size(600, 900);
            if (!Connect("127.0.0.1"))
            {
                Close();
                return;
            }
            Thread t1 = new Thread(TalkToServer);
            t1.Start();
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }
        }


        static public long GetCurrentTimeMS()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private void TalkToServer()
        {
            long previousTime = GetCurrentTimeMS();
            while (true)
            {
                long currentTime = GetCurrentTimeMS();
                if  (currentTime >= previousTime + 5)
                {
                    previousTime = currentTime;

                    SendDirection();
                    ReceiveEnvironment();
                }
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
            Byte[] buffer = new Byte[256];
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
            

            string display = String.Join('\n', new ArraySegment<string>(chunks, 4, chunks.Length - 4));

            Invoke(() => { richTextBox1.Text = display; });

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
                        pictureBox1.BackColor = Color.Transparent;
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = false;
                        pictureBox1.Location = new Point(200, 300);
                    });
                }
                else
                {
                    Invoke(() =>
                    {
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = true;
                        pictureBox1.Location = new Point(115, 300);
                        pictureBox2.Location = new Point(285, 300);
                    });
                    
                }

                Invoke(() =>
                {
                    switch (Int32.Parse(statement[statement.Length - 1]))
                    {
                        case 1:
                            pictureBox1.BackColor = Color.Red;
                            pictureBox1.Image = Properties.Resources._1;
                            break;
                        case 2:
                            pictureBox1.BackColor = Color.Red;
                            pictureBox1.Image = Properties.Resources._2;
                            break;
                        case 3:
                            pictureBox1.BackColor = Color.Red;
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
                            pictureBox1.Image = Properties.Resources._1;
                            pictureBox2.Image = Properties.Resources._9;
                            break;
                        case 20:
                            pictureBox1.Image = Properties.Resources._2;
                            pictureBox2.Image = Properties.Resources._0;
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

        List<PlayerBlockControl> pbs = new List<PlayerBlockControl>();
        void UpdatePlayerBlock(string[] players, string myName)
        {
            List<PlayerBlockControl> new_pbs = new List<PlayerBlockControl>();

            for (int i = 0; i < players.Length; i++)
            {
                string[] playerData = players[i].Split(',');
                PlayerBlock pb = new PlayerBlock(playerData);

                var temp = pbs.Find(x => x.pictureBox.Name == pb.name);
                if (temp != null)
                {
                    new_pbs.Add(temp);
                    Invoke(() =>
                    {
                        temp.pictureBox.Location = new Point(pb.x, pb.y);
                        temp.label.Location = new Point(pb.x, pb.y - 20);

                        temp.label.Text = "HP: " + Convert.ToString(pb.heart);
                    });

                    Graphics graphic = temp.pictureBox.CreateGraphics();
                    Font drawFont = new Font("Serif", 30);
                    SolidBrush drawBrush = new SolidBrush(Color.Black);
                    float x = 0.0F;
                    float y = 20.0F;
                    StringFormat drawFormat = new StringFormat();
                    drawFormat.LineAlignment = StringAlignment.Center;

                    Invoke(() => graphic.DrawString(pb.name, drawFont, drawBrush, x, y, drawFormat));
                }
                else
                {
                    var picture = new PictureBox();
                    var label = new Label();
                    new_pbs.Add(new PlayerBlockControl(picture, label));

                    Invoke(() => Controls.Add(picture));
                    Invoke(() => Controls.Add(label));
                    Invoke(() =>
                    {
                        picture.Size = new Size(pb.w, pb.h);
                        picture.Location = new Point(pb.x, pb.y);
                        picture.Name = pb.name;
                        picture.BackColor = (pb.name == myName ? Color.Aqua : Color.Black);

                        label.Text = "HP: " + Convert.ToString(pb.heart);
                        label.Size = new Size(70, 30);
                        label.Location = new Point(pb.x, pb.y - 20);
                    });
                }
            }

            foreach (PlayerBlockControl pb in pbs)
            {
                if (new_pbs.Find(x => x.pictureBox.Name == pb.pictureBox.Name) == null)
                {
                    Invoke(() =>
                    {
                        Controls.Remove(pb.pictureBox);
                        Controls.Remove(pb.label);
                        pb.pictureBox.Dispose();
                        pb.label.Dispose();
                    });

                }
            }

            pbs = new_pbs;
        }

        List<PictureBox> pfs = new List<PictureBox>();
        void UpdatePlatformBlock(string[] blocks)
        {
            if (blocks.Length == 0 || blocks[0] == String.Empty) return;

            List<PictureBox> new_pfs = new List<PictureBox>();

            for (int i = 0; i < blocks.Length; i++)
            {
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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

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
    public PictureBox pictureBox;
    public Label label;

    public PlayerBlockControl(PictureBox _pictureBox, Label _label)
    {
        pictureBox = _pictureBox;
        label = _label;
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