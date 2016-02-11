using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Microsoft.VisualBasic;

namespace Simples_Scanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region Imports
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);
        const int PROCESS_WM_READ = 0x0010;
        #endregion
        #region Variaveis
        static IntPtr HandleProcesso1;
        Process ProcessoUsar;
        bool Scaneando = false;
        #endregion
        #region Ferramentas
        struct Memoria
        {
            static public byte[] LerMemoria(int Endereco, int Tamanho = 4)
            {
                byte[] Bytes = new byte[Tamanho];
                int BytesLidos = 0;
                ReadProcessMemory((int)HandleProcesso1, Endereco, Bytes, Bytes.Length, ref BytesLidos);
                return Bytes;
            }
            static public void EscreverMemoria(int Endereco, byte[] Bytes, int Tamanho = 4)
            {
                int BytesEscritos = 0;
                WriteProcessMemory((int)HandleProcesso1, Endereco, Bytes, Bytes.Length, ref BytesEscritos);
            }

            static public string LerString(int Endereco, int Tamanho = 4)
            {
                return Encoding.ASCII.GetString(LerMemoria(Endereco, Tamanho));
            }
            static public int LerInt(int Endereco, int Tamanho = 4)
            {
                return BitConverter.ToInt32(LerMemoria(Endereco, Tamanho), 0);
            }
            static public float LerFloat(int Endereco, int Tamanho = 4)
            {
                return BitConverter.ToSingle(LerMemoria(Endereco, Tamanho), 0);
            }
            static public double LerDouble(int Endereco, int Tamanho = 4)
            {
                return BitConverter.ToDouble(LerMemoria(Endereco, Tamanho), 0);
            }
            static public void EscreverString(int Endereco, string Str)
            {
                byte[] BytesEscrever = Encoding.ASCII.GetBytes(Str);
                EscreverMemoria(Endereco, BytesEscrever, Str.Length * 2);
            }
            static public void EscreverInt(int Endereco, int Valor, int Tamanho = 4)
            {
                byte[] BytesEscrever = BitConverter.GetBytes(Valor);
                EscreverMemoria(Endereco, BytesEscrever, Tamanho);
            }
            static public void EscreverFloat(int Endereco, float Valor, int Tamanho = 4)
            {
                byte[] BytesEscrever = BitConverter.GetBytes(Valor);
                EscreverMemoria(Endereco, BytesEscrever, Tamanho);
            }
            static public void EscreverDouble(int Endereco, double Valor, int Tamanho = 4)
            {
                byte[] BytesEscrever = BitConverter.GetBytes(Valor);
                EscreverMemoria(Endereco, BytesEscrever, Tamanho);
            }
        }
        void Desabilitar()
        {
            this.Invoke((MethodInvoker)(() => button2.Enabled = false));
            this.Invoke((MethodInvoker)(() => button3.Enabled = false));
        }
        void Limpar()
        {
            this.Invoke((MethodInvoker)(() => listBox1.Items.Clear()));
        }
        void Habilitar()
        {
            this.Invoke((MethodInvoker)(() => button2.Enabled = true));
            this.Invoke((MethodInvoker)(() => button3.Enabled = true));
        }
        #endregion
        #region Funcoes
        void CarregarHandle()
        {
            HandleProcesso1 = OpenProcess(0x1F0FFF, false, ProcessoUsar.Id);
        }
  
        void ThreadProximoScanearInt(string[] Enderecos, int Value)
        {
            Desabilitar();
                            Limpar();
                Scaneando = true;
                foreach (string sEndereco in Enderecos)
                {
                    if (!Scaneando)
                    {
                        Habilitar();
                        return;
                    }
                int Endereco = int.Parse(sEndereco.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                int ValorDoEndereco = Memoria.LerInt(Endereco, BitConverter.GetBytes(Value).Length);
                    if (ValorDoEndereco == Value)
                    {
                        this.Invoke((MethodInvoker)(() => listBox1.Items.Add("0x" + Endereco.ToString("X"))));
                    }
                }
          
            Habilitar();
        }
        void ThreadProximoScanearString(string[] Enderecos, string Value)
        {
            Desabilitar();
            try
            {
                Limpar();
                Scaneando = true;
                foreach (string sEndereco in Enderecos)
                {
                    if (!Scaneando)
                    {
                        Habilitar();
                        return;
                    }
                    int Endereco = int.Parse(sEndereco, System.Globalization.NumberStyles.HexNumber);
                    string ValorDoEndereco = Memoria.LerString(Endereco, Value.Length);
                    if (ValorDoEndereco == Value)
                    {
                        this.Invoke((MethodInvoker)(() => listBox1.Items.Add("0x" + Endereco.ToString("X"))));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um error, por favor envie esse error para o gusdnide o mais rapido possivel, Contato Skype: bielzaao \n O Error Está ja foi copiado, simplesmente envie para o gusdnide ou cole em um bloco de notas e poste em algum dos foruns que gusdnide está e marque ele");
                Clipboard.SetText(ex.Message);
            }
            Habilitar();
        }
        void ThreadNovoScannerInt(int Value)
        {
            Desabilitar();
            try
            {
                Limpar();
                int Inicio = (int)Convert.ToInt64(textBox2.Text, 16);
                int Fim = (int)Convert.ToInt64(textBox3.Text, 16);
                Scaneando = true;
                for (int Endereco = Inicio; Endereco < ((Fim)); Endereco++)
                {
                    if (!Scaneando)
                    {
                        Habilitar();
                        return;
                    }
                    int ValorDoEndereco = Memoria.LerInt(Endereco, BitConverter.GetBytes(Value).Length);
                    if (ValorDoEndereco == Value)
                    {
                        this.Invoke((MethodInvoker)(() => listBox1.Items.Add("0x" + Endereco.ToString("X"))));

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um error, por favor envie esse error para o gusdnide o mais rapido possivel, Contato Skype: bielzaao \n O Error Está ja foi copiado, simplesmente envie para o gusdnide ou cole em um bloco de notas e poste em algum dos foruns que gusdnide está e marque ele");
                Clipboard.SetText(ex.Message);
            }
            Habilitar();
        }
        void ThreadNovoScanearString(string Value)
        {
            Desabilitar();
            try
            {
                Limpar();
                int Inicio = (int)Convert.ToInt64(textBox2.Text, 16);
                int Fim = (int)Convert.ToInt64(textBox3.Text, 16);
                Scaneando = true;
                for (int Endereco = Inicio; Endereco < ((Fim)); Endereco++)
                {
                    if (!Scaneando)
                    {
                        Habilitar();
                        return;
                    }
                    string ValorDoEndereco = Memoria.LerString(Endereco, Value.Length);
                    if (ValorDoEndereco == Value)
                    {

                        this.Invoke((MethodInvoker)(() => listBox1.Items.Add("0x" + Endereco.ToString("X"))));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um error, por favor envie esse error para o gusdnide o mais rapido possivel, Contato Skype: bielzaao \n O Error Está ja foi copiado, simplesmente envie para o gusdnide ou cole em um bloco de notas e poste em algum dos foruns que gusdnide está e marque ele");
                Clipboard.SetText(ex.Message);
            }
            Habilitar();
        }
        #endregion
        #region CriarThreads
        void tNovoScan(int Tipo, object Valor)
        {
            Thread TSc;
            if (Tipo == 0)
            {
                int Value = Convert.ToInt32(Valor);
                TSc = new Thread(() => ThreadNovoScannerInt(Value));

            }
            else
            {
                string Value = Convert.ToString(Valor);
                TSc = new Thread(() => ThreadNovoScanearString(Value));
            }

            TSc.Start();
        }
        void tProximoScanear(int Tipo, string[] Enderecos, object Valor)
        {

            Thread TSc;
            if (Tipo == 0)
            {
                int Value = Convert.ToInt32(Valor);
                TSc = new Thread(() => ThreadProximoScanearInt(Enderecos, Value));
            }
            else
            {
                string Value = Convert.ToString(Valor);
                TSc = new Thread(() => ThreadProximoScanearString(Enderecos, Value));
            }
            TSc.Start();
        }
        void NovoScanner()
        {
            button3.Enabled = true;
            if (comboBox1.SelectedIndex > -1)
                tNovoScan(comboBox1.SelectedIndex, textBox1.Text);
        }
        void ProximoScanner()
        {
            string[] Enderecos = new string[listBox1.Items.Count];
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                Enderecos[i] = listBox1.Items[i].ToString();
            }
            if (comboBox1.SelectedIndex > -1)
                tProximoScanear(comboBox1.SelectedIndex, Enderecos, textBox1.Text);
        }
        #endregion


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NovoScanner();
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Limpar();
            SelecProc t = new SelecProc();
            t.ShowDialog();
            if (!t.Selecionado) return;
            ProcessoUsar = t.ProcSelecionado;
            CarregarHandle();
            this.Text = "TGH Simples Scan - " + ProcessoUsar.ProcessName;
            groupBox2.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                int Endereco = (int)Convert.ToInt64(listBox1.SelectedItem.ToString(), 16);
                object Value = Interaction.InputBox("Digite o Value", "Descrição", "", 100, 200);
                if (comboBox1.SelectedIndex == 0)
                {
                    Memoria.EscreverInt(Endereco, Convert.ToInt32(Value));
                }
                else
                {
                    Memoria.EscreverString(Endereco, Convert.ToString(Value));
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listBox1.Items.Count < 1)
                contextMenuStrip1.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProximoScanner();
        }
    }
}
