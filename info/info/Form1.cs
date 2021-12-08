using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace info
{
    public partial class Form1 : Form
    {
        private string text_inicio = "";
        private string RunningPath = AppDomain.CurrentDomain.BaseDirectory; // BASE DO DIRETÓRIO RAIZ (ONDE O PROGRAMA ESTÁ INSTALADO NO SISTEMA OPERACINAL)

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.text_inicio = txtManual.Text; // RESERVA VALOR PADRÃO
            try
            {
                string[] dirs = Directory.GetDirectories(RunningPath+"/docs/", "*"); // SCANEANDO INDEX (PASTAS)

                int i = 1;
                foreach (string dir in dirs)
                {
                    tvwTitulos.Nodes.Add(dir.Replace(RunningPath + "/docs/", "")); // CRIANDO NODE NO TREEVIEW

                    string[] files = Directory.GetFiles(dir, "*"); // SCANEANDO FILES (MANUAIS DE INSTRUÇÃO)
                    foreach (string file in files)
                    {
                        tvwTitulos.Nodes[i].Nodes.Add("* " + file.Replace(dir+@"\", "").Replace(".txt", "")); // CRIANDO CHILD
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // Form1_Load()

        private void tvwTitulos_AfterSelect(object sender, TreeViewEventArgs e) // SELECIONADO MANUAL DE INSTRUÇÃO PARA LEITURA
        {
            string node_text = tvwTitulos.SelectedNode.Text;

            if (node_text == "Inicio") // PAGINA DE APRESENTAÇÃO DA DOCUMANTAÇÃO
            {
                foreach (Control item in txtManual.Controls.OfType<LinkLabel>().ToList()) // REMOVENDO LINKLABELS
                {
                    txtManual.Controls.Remove(item);
                }
                txtManual.Text = this.text_inicio; // RETORNA AO TEXTO VALOR PADRÃO
            }
            else
            {
                if (node_text.IndexOf("*") != -1)
                {
                    string[] dirs = Directory.GetDirectories(RunningPath + "/docs/", "*");

                    foreach (string dir in dirs)
                    {
                        string[] files = Directory.GetFiles(dir, "*");
                        foreach (string file in files)
                        {
                            string filename = file.Replace(dir, "").Replace("\\", "").Replace(".txt", "");
                            string node_selected = node_text.Replace("* ", "");

                            if (filename == node_selected)
                            {
                                foreach (Control item in txtManual.Controls.OfType<LinkLabel>().ToList())  // REMOVENDO LINKLABELS
                                {
                                    txtManual.Controls.Remove(item);
                                }

                                txtManual.Text = getTextFile(file); // OBTENDO CONTEÚDO DO ARQUIVO DE TEXTO
                                break;
                            }
                        }
                    }
                }
            }
        } // tvwTitulos_AfterSelect()

        private string getTextFile(string fileName) // LÊ CONTEÚDO DE ARQUIVO '.TXT'
        {
            string text = "";

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    text += line + "\n";
                }
            }

            return text;
        } // getTextFile()

        private void Pesquisar()
        {
            string pesquisa = txtPesquisa.Text;

            ArrayList resultados = new ArrayList();

            string[] dirs = Directory.GetDirectories(RunningPath + "/docs/", "*"); // SCANEANDO INDEX (PASTAS)

            foreach (string dir in dirs)
            {
                string[] files = Directory.GetFiles(dir, "*"); // SCANEANDO FILES (MANUAIS DE INSTRUÇÃO)
                foreach (string file in files)
                {
                    string filename = file.Replace(dir, "").Replace("\\", "").Replace(".txt", "");

                    if (filename.ToLower().IndexOf(pesquisa.ToLower()) != -1)
                    {
                        string[] results = new string[] {
                            "* " + filename,
                            file
                        };

                        resultados.Add(results);
                    }
                    else
                    {
                        string text = getTextFile(file); // OBTENDO CONTEÚDO DO ARQUIVO DE TEXTO

                        if (text.ToLower().IndexOf(pesquisa.ToLower()) != -1)
                        {
                            string[] results = new string[] {
                            "* " + filename,
                            file
                        };

                            if (!resultados.Contains(results))
                                resultados.Add(results);
                        }
                    }
                }
            }

            if (resultados.Count == 0) // QUANDO NÃO ACHOU NENHUM ASSUNTO RELACIONADO A PESQUISA
            {
                foreach (Control item in txtManual.Controls.OfType<LinkLabel>().ToList())
                {
                    txtManual.Controls.Remove(item);
                }

                txtManual.Text = "\n\tNenhum resultado encontrado!";
            }
            else
            {
                foreach (Control item in txtManual.Controls.OfType<LinkLabel>().ToList())  // REMOVENDO LINKLABELS
                {
                    txtManual.Controls.Remove(item);
                }

                txtManual.Text = "\n\tRESULTADOS (" + resultados.Count + "):\n\n";
                foreach (string[] item in resultados)
                {
            
                    LinkLabel link = new LinkLabel();
                    link.Text = item[0];
                    link.AutoSize = true;

                    link.Location = txtManual.GetPositionFromCharIndex(txtManual.TextLength);
                    txtManual.Controls.Add(link);

                    txtManual.AppendText(link.Text + "   ");
                    txtManual.SelectionStart = txtManual.TextLength;

                    txtManual.Text += "\n\n"+getTextFile(item[1])+"\n\n";
                }
            }

            txtPesquisa.Clear();
        } // Pesquisar

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            if(txtPesquisa.Text.Trim() != "")
            {
                Pesquisar();
            }
            else
            {
                MessageBox.Show("Digite sua pesquisa", "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPesquisa.Focus();
            }
        } // btnPesquisar_Click()

        private void txtPesquisa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13)
            {
                if (txtPesquisa.Text.Trim() != "")
                {
                    Pesquisar();
                }
                else
                {
                    MessageBox.Show("Digite sua pesquisa", "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPesquisa.Focus();
                }
            }
        }
    } // txtPesquisa_KeyPress()
}
