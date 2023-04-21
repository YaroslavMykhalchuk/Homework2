using System.Diagnostics;
using System.Management;

namespace Homework2
{
    public partial class MainForm : Form
    {
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            getDictionary();
            CreateNodes();
        }

        private void getDictionary()
        {

            var bufproc = Process.GetProcesses();
            foreach (var proc in bufproc)
            {
                try
                {
                    dictionary.Add(proc.Id, getParent(proc.Id));
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private int getParent(int childProc)
        {
            int parentid = 0;
            using (ManagementObject obj = new ManagementObject($"win32_process.handle={childProc}"))
            {
                obj.Get();
                parentid = Convert.ToInt32(obj["ParentProcessId"]);
            }
            return parentid;
        }
        private void CreateNodes()
        {
            int count = dictionary.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var pair = dictionary.ElementAt(i);
                    if (pair.Key == pair.Value || !dictionary.ContainsKey(pair.Value))
                    {
                        Process process = Process.GetProcessById(pair.Key);
                        TreeNode node = new TreeNode($"{process.ProcessName} [{process.Id}],parent:[{pair.Value}]");
                        node.Tag = process;
                        treeView.Nodes.Add(node);
                    }
                }
                catch { }
            }
            GetSubNotes(treeView.Nodes);
        }
        private void GetSubNotes(TreeNodeCollection treeNodeCollection)
        {
            try
            {
                int countNode = 0;
                foreach (TreeNode node in treeNodeCollection)
                {
                    Process? process = node.Tag as Process;
                    int count = dictionary.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var pair = dictionary.ElementAt(i);
                        if (process!.Id == pair.Value)
                        {
                            Process process1 = Process.GetProcessById(pair.Key);
                            TreeNode node2 = new TreeNode($"{process1.ProcessName} [{process1.Id}],parent:[{pair.Value}]");
                            node2.Tag = process;
                            node.Nodes.Add(node2);
                            i--;
                            count--;
                            countNode++;
                            dictionary.Remove(pair.Key);
                        }
                    }
                }
                if (countNode > 0)
                {
                    foreach (TreeNode node in treeNodeCollection)
                    {
                        GetSubNotes(node.Nodes);
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}