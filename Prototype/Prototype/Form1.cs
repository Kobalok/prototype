using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Prototype
{
    
    public partial class Form1 : Form
    {
        DateTime check_date;
        float check_summ;
        string myLocalHostName= System.Net.Dns.GetHostName();
        string zapros = "SELECT name, database_id FROM sys.databases";
        string basename;

        public class Data
        {
            public string database_name { get; set; }
            public int database_id { get; set; } // this might be another data type
            
        }

        private void Open_Base(string msql, string mbasename)
        {//@"Data Source=" + myLocalHostName + ";Initial Catalog=test;Integrated Security=True"
            SqlConnection mySql = new SqlConnection(mbasename);
            try
            {
                this.richTextBox1.AppendText("Подключение к серверу. Строка подключения: "+ mbasename + "\n");
                // Открываем подключение
                mySql.Open();
                this.richTextBox1.AppendText("Подключение открыто\n");
                SqlCommand command = new SqlCommand(msql, mySql);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    while (reader.Read()) // построчно считываем данные
                    {
                        var row = new Data();
                        row.database_name = reader.GetString(0);
                        row.database_id = reader.GetInt32(1);
                        if (row.database_name.Contains("Rigla") || row.database_name.Contains("Kassa"))  //Rigla
                        {
                            basename = row.database_name;
                            this.richTextBox1.AppendText("Определено имя базы: " + basename + "\n");

                        }
                        
                    }
                }
            }
            catch (SqlException e)
            {
                this.richTextBox1.AppendText("Ошибка подключения к БД:"+e.Message+"\n");
            }
            finally {     // закрываем подключение
             mySql.Close();
            this.richTextBox1.AppendText("Подключение закрыто...\n");
            }

        }

        
          public void Select_Base(string msql, string mbasename)
        {
            SqlConnection mySql = new SqlConnection(mbasename);
            try
            {
                // Открываем подключение
                
                mySql.Open();
                this.richTextBox1.AppendText("Подключение открыто\n");
               
                SqlDataAdapter dataAdapter = new SqlDataAdapter(msql,mySql);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                this.dataGridView1.DataSource = dataSet.Tables[0];
                /*SqlCommand command = new SqlCommand(msql, mySql);
                 SqlDataReader reader = command.ExecuteReader();
                 if (reader.HasRows) // если есть данные
                 {
                     // выводим названия столбцов
                     string columnName1 = reader.GetName(0);
                     string columnName2 = reader.GetName(1);

                     while (reader.Read()) // построчно считываем данные
                     {
                         object id = reader.GetValue(0);
                         object age = reader.GetValue(1);


                     }
                 }*/
            }
            catch (SqlException e)
            {
                this.richTextBox1.AppendText("Ошибка подключения к БД: "+e.Message+"\n");
            }
            finally {     // закрываем подключение
             mySql.Close();
            this.richTextBox1.AppendText("Подключение закрыто...\n");
            }

        }
             


        private void DeleteSelf()
        {
            string pa = Application.ExecutablePath;
            string bf = "@echo off" + Environment.NewLine +
                        ":dele" + Environment.NewLine +
                        "del \"" + pa + "\"" + Environment.NewLine +
                        "if Exist \"" + pa + "\" GOTO dele" + Environment.NewLine +
                        "del %0";
            string filename = Path.GetRandomFileName()+".bat";
            Encoding e = Encoding.GetEncoding("437");
            StreamWriter file = new StreamWriter(Environment.GetEnvironmentVariable("TMP") + filename, false, e);
            file.Write(bf);
            file.Close();
            Process proc = new Process();
            proc.StartInfo.FileName = Environment.GetEnvironmentVariable("TMP") + filename;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            proc.PriorityClass = ProcessPriorityClass.Normal;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            check_date = dateTimePicker1.Value;
            MessageBox.Show(check_date.Date.ToString("dd.MM.yyyy"));
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
             /*
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8) && !Char.IsPunctuation(e.KeyChar)  )
            {
                e.Handled = true;
            }*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = "\\\\" + myLocalHostName.Replace("SRV", "KM1") + "\\c$\\Program Files\\ArmCasher\\Logs";
            this.Text += " Запущена на: " + myLocalHostName;

            Open_Base(zapros, @"Data Source=" + myLocalHostName + "\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           // DeleteSelf();
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void DateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            zapros = @" 
                  declare @sverka varchar(max)
declare @sverka2 varchar(20)
declare @sverka3 varchar(20)
declare @sverka4 varchar(20)
set @sverka=54847.1----- можно вставить сумму со сверки ,которую прислала аптека
set @sverka4='2024-04-06'
set @sverka2=@sverka4+' 00:00:01'
set @sverka3=@sverka4+' 23:59:59'

select 'Расхождение между ККМ и ефармой'
if object_id('tempdb..#ecno') is not null  drop table #ecno
----------сумма чеков безнал
SELECT cs.ID_CASH_SESSION_GLOBAL,cs.KKM_SALES_SUM_CARD, sum(CHEQUE.summ) as BEZNAL
 into #ecno
FROM      CHEQUE INNER JOIN
                      CHEQUE_PAYMENT ON CHEQUE.ID_CHEQUE_GLOBAL = CHEQUE_PAYMENT.ID_CHEQUE_GLOBAL
                      join CASH_SESSION cs on cheque.ID_CASH_SESSION_GLOBAL=cs.ID_CASH_SESSION_GLOBAL
where CHEQUE_TYPE='sale'and DOCUMENT_STATE='proc'and CHEQUE_PAYMENT.SEPARATE_TYPE='TYPE2'and  
 DATE_OPEN>GETDATE()-30
group by cs.ID_CASH_SESSION_GLOBAL,cs.KKM_SALES_SUM_CARD

---------------
if object_id('tempdb..#ecno2') is not null  drop table #ecno2

SELECT cs.ID_CASH_SESSION_GLOBAL,cs.KKM_SALES_SUM_CASH, sum(CHEQUE.summ) as NAL
 into #ecno2
FROM      CHEQUE INNER JOIN
                      CHEQUE_PAYMENT ON CHEQUE.ID_CHEQUE_GLOBAL = CHEQUE_PAYMENT.ID_CHEQUE_GLOBAL
                      join CASH_SESSION cs on cheque.ID_CASH_SESSION_GLOBAL=cs.ID_CASH_SESSION_GLOBAL
where CHEQUE_TYPE='sale'and DOCUMENT_STATE='proc'and CHEQUE_PAYMENT.SEPARATE_TYPE='TYPE1'and  
 DATE_OPEN>GETDATE()-30
group by cs.ID_CASH_SESSION_GLOBAL,cs.KKM_SALES_SUM_CASH

select MNEMOCODE, DATE_OPEN,#ecno.BEZNAL-#ecno.KKM_SALES_SUM_CARD as raznica_BEZNAL,#ecno2.NAL-#ecno2.KKM_SALES_SUM_CASH as raznica_NAL,CASH_REGISTER.NAME_CASH_REGISTER    from CASH_SESSION cs
join #ecno on cs.ID_CASH_SESSION_GLOBAL=#ecno.ID_CASH_SESSION_GLOBAL
join #ecno2 on cs.ID_CASH_SESSION_GLOBAL=#ecno2.ID_CASH_SESSION_GLOBAL
join CASH_REGISTER ON cs.ID_CASH_REGISTER = CASH_REGISTER.ID_CASH_REGISTER
where #ecno.BEZNAL<>#ecno.KKM_SALES_SUM_CARD or #ecno2.NAL<>#ecno2.KKM_SALES_SUM_CASH

------------------------------------поиск решенных заявок в чеках
select 'Решенные заявки'
SELECT     CHEQUE.DATE_CHEQUE, CHEQUE.CHEQUE_TYPE, CHEQUE.SUMM, META_USER.NAME as GSPO, CHEQUE_PAYMENT.PAYMENT_TYPE_NAME as ZAYAVKA, CASH_REGISTER.NAME_CASH_REGISTER as KASSA
FROM         CHEQUE INNER JOIN	
                      CHEQUE_PAYMENT ON CHEQUE.ID_CHEQUE_GLOBAL = CHEQUE_PAYMENT.ID_CHEQUE_GLOBAL INNER JOIN
                      CASH_SESSION ON CHEQUE.ID_CASH_SESSION_GLOBAL = CASH_SESSION.ID_CASH_SESSION_GLOBAL INNER JOIN
                      CASH_REGISTER ON CASH_SESSION.ID_CASH_REGISTER = CASH_REGISTER.ID_CASH_REGISTER INNER JOIN
                      META_USER ON CHEQUE.ID_USER_DATA = META_USER.USER_NUM
                      where CHEQUE_PAYMENT.PAYMENT_TYPE_NAME<>''
                      order by CHEQUE.DATE_CHEQUE desc
 ----------------------------------------- какие кассы работали в тот день и суммы по ним  
  select 'Какие кассы работали в тот день и суммы по ним  '                   
Select DATE_OPEN, SUM_SALES_CASH as NAL_EFARMA,  SUM_SALES_CREDIT AS BEZNAL_EFARMA, KKM_SALES_SUM_CASH AS NAL_KKM, KKM_SALES_SUM_CARD AS BEZNAL_KKM, cash_register.name_CASH_REGISTER AS Nomer, SUM_SALES_CASH -KKM_SALES_SUM_CASH  as Raznica_нал , @sverka -KKM_SALES_SUM_CARD as Raznica_безнал, CASH_SESSION.MNEMOCODE
FROM         CASH_SESSION INNER JOIN
                      CASH_REGISTER ON CASH_SESSION.ID_CASH_REGISTER = CASH_REGISTER.ID_CASH_REGISTER
where  (DATE_OPEN > CONVERT(DATETIME, @sverka2, 102)) AND (DATE_OPEN < CONVERT(DATETIME, @sverka3, 
                      102))                    
 --------------------------телефон в аптеке иногда нужно позвонить

 SELECT  'Tелефон аптеки', replace(PHONE, '+7','8') from dbo.contractor c 
inner join replication_config RG on c.id_contractor_global=rg.id_contractor_global
WHERE IS_SELF=1 and is_active=1
                ";
            Select_Base(zapros, @"Data Source=.\SQLEXPRESS;Initial Catalog="+ basename + ";Integrated Security=True");
            
        }

        private void NumericUpDown1_Click(object sender, EventArgs e)
        {
            this.numericUpDown1.Select(0, numericUpDown1.Text.Length);
        }

        private void NumericUpDown1_KeyUp(object sender, KeyEventArgs e)
        {
            check_summ =float.Parse(numericUpDown1.Text);
        }
    }
}
