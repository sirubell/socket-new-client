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

            string display = String.Join('\n', new ArraySegment<string>(chunks, 4, chunks.Length - 4));

            Invoke(() => { richTextBox1.Text = display; });

            UpdatePlayerBlock(players, myName);
            UpdatePlatformBlock(blocks);
        }

        List<PlayerBlockControl> pbs = new List<PlayerBlockControl>();
        void UpdatePlayerBlock(string[] players, string myName)
        {
            while (pbs.Count < players.Length)
            {
                var picture = new PictureBox();
                var label = new Label();

                pbs.Add(new PlayerBlockControl(picture, label));

                Invoke( () =>
                {
                    Controls.Add(picture);
                    Controls.Add(label);
                });
            }
            while (pbs.Count > players.Length)
            {
                var control = pbs.Last();
                Invoke(() =>
                {
                    Controls.Remove(control.pictureBox);
                    Controls.Remove(control.label);
                });
                pbs.RemoveAt(pbs.Count - 1);
            }

            for (int i = 0; i < pbs.Count; i++)
            {
                string[] playerData = players[i].Split(',');
                PlayerBlock pb = new PlayerBlock(playerData);

                Invoke(() =>
                {
                    pbs[i].pictureBox.Size = new Size(pb.w, pb.h);
                    pbs[i].pictureBox.Location = new Point(pb.x, pb.y);
                    pbs[i].pictureBox.BackColor = (pb.name == myName ? Color.Aqua : Color.Black);

                    pbs[i].label.Text = pb.heart;
                    pbs[i].label.Size = new Size(100, 30);
                    pbs[i].label.Location = new Point(pb.x, pb.y - 20);
                });
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