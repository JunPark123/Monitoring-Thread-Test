using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Thread _thread;
        public Thread _checkthread;

        public delegate void EHandle();
        public EHandle monitoringValue;

        private int m_value;
        private Recv recv;

        private bool isEventSuccess = false; // 이벤트 성공 여부 플래그
        public Form1()
        {
            InitializeComponent();
            m_value = 0;
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            recv = new Recv();
            recv.MyEvent += EventHandler; // 이벤트 핸들러 등록                                         
            _thread = new Thread(recvThread); // 별도의 스레드에서 recv.Start() 실행
            //_checkthread = new Thread(new ThreadStart(checkThread)); //checkthread 선언
            monitoringValue = new EHandle(OnSomeEvent);

            _thread.Start();
            //_checkthread.Start();
        }

        public void recvThread()
        {
            while (true)
            {
                recv.start();
                Thread.Sleep(500); // 1초 대기
            }
        }

        public void checkThread()
        {
            while(true)
            {
                this.Invoke(monitoringValue);
                Thread.Sleep(500);
            }
        }

        private void EventHandler(string message)
        {
            //여기서 메세지를 전달받는다
            //전달받은 메세지를 리스트박스에 계속 추가한다.
            // UI 스레드에서 리스트 박스에 추가
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new Action(() => 
                {
                    OnSomeEvent();
                    listBox1.Items.Add(message);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    
                }));               
                m_value = int.Parse(message.Substring(5));               
            }
            else
            {
                listBox1.Items.Add(message);
            }

           

        }

        private void OnSomeEvent()
        {
            if (m_value == 33)
            {
                isEventSuccess = true;
                button1.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                isEventSuccess = true;
                button1.BackColor = System.Drawing.Color.Red;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_thread!=null)
            {
                _thread.Abort();
                _thread.Join();

                _checkthread.Abort();
                _checkthread.Join();
            }
        }
    }

    public class EventClass
    {
        public string message = "";
        private EventHandler handler;
        public event EventHandler someEvent
        {
            add
            {

                handler += value;
            }
            remove
            {

                handler -= value;
            }
        }

        public void doEvent()
        {
            handler?.Invoke(this, EventArgs.Empty);
        }
    }

    public class Recv
    {
        public delegate void RecvEvent(string message);

        public event RecvEvent MyEvent;

        public void start()
        {
            int a = creatRandomVal();
            MyEvent?.Invoke("값은 : " + a.ToString());
            // 일정 시간 동안 대기 혹은 다른 조건을 검사하여 스레드를 중단할 수 있음
        }

        public int creatRandomVal()
        {
            Random random = new Random();
            int ran = 0;
            ran = random.Next(30,40);
            return ran;
        }
    }
}
