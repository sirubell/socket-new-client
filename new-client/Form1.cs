using System.Net.Sockets;
using System.Drawing;

namespace new_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            if (!Connect("127.0.0.1"))
            {
                Close();
            }
        }
        private void Form1_Closing(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(TalkToServer);
            t1.Start();
            button1.Visible = false;
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
            if (textBox1.Text == String.Empty) return;

            Byte[] data = System.Text.Encoding.ASCII.GetBytes(textBox1.Text);
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

            string display = String.Join('\n', new ArraySegment<string>(chunks, 4, chunks.Length - 4));

            Invoke(() => { richTextBox1.Text = display; });

            UpdatePlayerBlock(players);
            UpdatePlatformBlock(blocks);
        }

        void UpdatePlayerBlock(string[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                string[] playerData = players[i].Split(',');
                PlayerBlock pb = new PlayerBlock(playerData);

                try
                {
                    PictureBox pictureBox = (PictureBox)Controls.Find($"player{i}", true)[0];
                    Invoke(() => {
                        pictureBox.Size = new Size(pb.w, pb.h);
                        pictureBox.Location = new Point(pb.x, pb.y);
                    });
                    
                }
                catch (Exception ex)
                {
                    var picture = new PictureBox
                    {
                        Name = $"player{i}",
                        Size = new Size(pb.w, pb.h),
                        Location = new Point(pb.x, pb.y),
                        BackColor = Color.Black,
                    };

                    Invoke(() => { Controls.Add(picture); });
                };
            }
        }

        void UpdatePlatformBlock(string[] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                string[] blockData = blocks[i].Split(',');
                FlatformBlock pf = new FlatformBlock(blockData);

                try
                {
                    PictureBox flatformBlock = (PictureBox)Controls.Find($"block{i}", true)[0];
                    Invoke(() => {
                        flatformBlock.Size = new Size(pf.w, pf.h);
                        flatformBlock.Location = new Point(pf.x, pf.y);
                    });

                }
                catch (Exception ex)
                {
                    var picture = new PictureBox
                    {
                        Name = $"block{i}",
                        Size = new Size(pf.w, pf.h),
                        Location = new Point(pf.x, pf.y),
                        BackColor = Color.Blue,
                    };

                    Invoke(() => { Controls.Add(picture); });
                };
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
    public int heart;
    public string name;

    public PlayerBlock(string[] data)
    {
        x = Convert.ToInt32(data[0]);
        y = Convert.ToInt32(data[1]);
        w = Convert.ToInt32(data[2]);
        h = Convert.ToInt32(data[3]);
        heart = Convert.ToInt32(data[4]);
        name = data[5];
    }
}

class FlatformBlock
{
    public int x;
    public int y;
    public int w;
    public int h;
    public int type;

public FlatformBlock(string[] data)
{
    x = Convert.ToInt32(data[0]);
    y = Convert.ToInt32(data[1]);
    w = Convert.ToInt32(data[2]);
    h = Convert.ToInt32(data[3]);
    type = Convert.ToInt32(data[4]);
}
}