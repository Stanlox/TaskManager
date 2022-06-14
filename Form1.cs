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
using System.Management;
using Microsoft.VisualBasic;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        private List<Process> processes;
        private ListViewItemComparator comparator;
        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            processes.Clear();
            processes = Process.GetProcesses().ToList<Process>();
        }

        private void RefreshProcessesList(List<Process> processes, string keyword)
        {
            try
            {
                listView1.Items.Clear();
                double memorySize = 0;
                foreach (Process process in processes)
                {
                    if (process != null)
                    {
                        PerformanceCounter performanceCounter = new PerformanceCounter();
                        performanceCounter.CategoryName = "Process";
                        performanceCounter.CounterName = "Working Set - Private";
                        performanceCounter.InstanceName = process.ProcessName;

                        memorySize = (double)performanceCounter.NextValue() / (1000 * 1000);
                        string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memorySize, 1).ToString() };
                        listView1.Items.Add(new ListViewItem(row));


                        performanceCounter.Close();
                        performanceCounter.Dispose();
                    }
                }

                Text = $"Запущено процессов {keyword} " + processes.Count.ToString();
            }
            catch (Exception) { }
        }

        private void RefreshProcessesList()
        {
            listView1.Items.Clear();
            double memorySize = 0;
            foreach (Process process in processes)
            {
                PerformanceCounter performanceCounter = new PerformanceCounter();
                performanceCounter.CategoryName = "Process";
                performanceCounter.CounterName = "Working Set - Private";
                performanceCounter.InstanceName = process.ProcessName;

                memorySize = (double)performanceCounter.NextValue() / (1000 * 1000);
                string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memorySize, 1).ToString() };
                listView1.Items.Add(new ListViewItem(row));


                performanceCounter.Close();
                performanceCounter.Dispose();
            }

            Text = "Запущено процессов: " + processes.Count.ToString();

        }


        private void KillProcess(Process process)
        {
            process.Kill();
            process.WaitForExit();
        }

        private void killProcessAndChildren(int processId)
        {
            if (processId == 0)
            {
                return;
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + processId);
            ManagementObjectCollection objectCollection = searcher.Get();
            foreach (ManagementObject obj in objectCollection)
            {
                killProcessAndChildren(Convert.ToInt32(obj["ProcessID"]));
            }

            try
            {
                Process p = Process.GetProcessById(processId);
                p.Kill();
                p.WaitForExit();
            }
            catch (ArgumentException) { }
        }

        private int GetParentProcessId(Process p)
        {
            int parentID = 0;
            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + p.Id + "'");
                managementObject.Get();
                parentID = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception) { }

            return parentID;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();
            GetProcesses();
            RefreshProcessesList();
            comparator = new ListViewItemComparator();
            comparator.ColumnIndex = 0;
        }

        private void Update_Click(object sender, EventArgs e)
        {
            GetProcesses();
            RefreshProcessesList();
        }

        private void CloseTask_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProcess(processToKill);
                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch(Exception) { }
        }

        private void CloseThreadOfProcesses_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    killProcessAndChildren(GetParentProcessId(processToKill));
                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void CloseThreadOfProcessesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    killProcessAndChildren(GetParentProcessId(processToKill));
                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void CreateNewProcesses_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы", "Запуск новой задачи");

            try
            {
                Process.Start(path);
            }
            catch(Exception) { }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();

            List<Process> filterProcesses = processes.Where((x) => x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();
            RefreshProcessesList(filterProcesses, toolStripTextBox1.Text);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparator.ColumnIndex = e.Column;
            comparator.SortDirection = comparator.SortDirection == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            listView1.ListViewItemSorter = comparator;
            listView1.Sort();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CloseProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProcess(processToKill);
                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }
    }
}
