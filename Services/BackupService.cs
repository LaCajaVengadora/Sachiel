using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sachiel.Services
{
    public class BackupService
    {
        private readonly string _databasePath;
        private readonly string _backupDirectory;

        public BackupService()
        {
            string databaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
            _databasePath = Path.Combine(databaseDirectory, "Sachiel.db");
            _backupDirectory = Path.Combine(databaseDirectory, "Backups");
        }

        public bool CreateBackup()
        {
            try
            {
                if (!File.Exists(_databasePath)) return false;
                Directory.CreateDirectory(_backupDirectory);

                string fileName = $"Sachiel_{DateTime.Now:yyMMdd_HHmmss}.db";
                string backupPath = Path.Combine(_backupDirectory, fileName);

                File.Copy(_databasePath, backupPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RestoreBackup(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath)) return false;
                if (!File.Exists(_databasePath)) return false;
                Directory.CreateDirectory(_backupDirectory);

                string preRestorePath = Path.Combine(_backupDirectory, "Sachiel_PreRestore.db");
                File.Copy(_databasePath, preRestorePath, true);
                File.Copy(backupPath, _databasePath, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
