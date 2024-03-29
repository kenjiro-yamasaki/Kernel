﻿using SoftCube.Asserts;
using SoftCube.System;
using System;
using System.IO;
using System.Text;

namespace SoftCube.Logger
{
    /// <summary>
    /// ファイルアペンダー。
    /// </summary>
    public class FileAppender : Appender
    {
        #region プロパティ

        /// <summary>
        /// ファイルパス。
        /// </summary>
        public string FilePath => FileStream.Name;

        /// <summary>
        /// ファイルサイズ（単位：byte）。
        /// </summary>
        public long FileSize => FileStream.Position;

        /// <summary>
        /// エンコーディング。
        /// </summary>
        public Encoding Encoding => Writer.Encoding;

        /// <summary>
        /// ファイルストリーム。
        /// </summary>
        private FileStream FileStream { get; set; }

        /// <summary>
        /// ストリームライター。
        /// </summary>
        private StreamWriter Writer { get; set; }

        #endregion

        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public FileAppender()
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        public FileAppender(ISystemClock systemClock)
            : base(systemClock)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="append">ファイルにログを追加するかを示す値。</param>
        /// <param name="encoding">エンコーディング。</param>
        /// <seealso cref="Open(string, bool, Encoding)"/>
        public FileAppender(string filePath, bool append, Encoding encoding)
            : this(new SystemClock(), filePath, append, encoding)
        {
        }

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="systemClock">システムクロック。</param>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="append">ファイルにログを追加するかを示す値。</param>
        /// <param name="encoding">エンコーディング。</param>
        /// <seealso cref="Open(string, bool, Encoding)"/>
        public FileAppender(ISystemClock systemClock, string filePath, bool append, Encoding encoding)
            : base(systemClock)
        {
            Open(filePath, append, encoding);
        }

        #endregion

        #region メソッド

        #region 破棄

        /// <summary>
        /// 破棄します。
        /// </summary>
        /// <param name="disposing">
        /// <see cref="IDisposable.Dispose"/> から呼び出されたかを示す値。
        /// <c>true</c> の場合、マネージリソースを破棄します。
        /// <c>false</c> の場合、マネージリソースを破棄しないでください。
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion

        #region ログ

        /// <summary>
        /// ログを出力します。
        /// </summary>
        /// <param name="log">ログ。</param>
        public override void Log(string log)
        {
            if (Writer == null)
            {
                return;
            }

            lock (Writer)
            {
                Writer.Write(log);
                Writer.Flush();
            }
        }

        #endregion

        /// <summary>
        /// ログファイルを開きます。
        /// </summary>
        /// <param name="filePath">ファイルパス。</param>
        /// <param name="append">
        /// ファイルにログを追加するかを示す値。
        /// <c>true</c> の場合、ファイルにログを追加します。
        /// <c>false</c> の場合、ファイルのログを上書きします。
        /// </param>
        /// <param name="encoding">エンコーディング。</param>
        public void Open(string filePath, bool append, Encoding encoding)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            Close();

            if (append)
            {
                FileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                FileStream.Seek(0, SeekOrigin.End);
            }
            else
            {
                FileStream = File.Create(filePath);
            }
            Writer = new StreamWriter(FileStream, encoding);
        }

        /// <summary>
        /// ログファイルを閉じます。
        /// </summary>
        public void Close()
        {
            if (FileStream != null && Writer != null)
            {
                lock (Writer)
                {
                    Writer.Dispose();
                    Writer = null;

                    FileStream.Dispose();
                    FileStream = null;
                }
            }
            else
            {
                Assert.Null(FileStream);
                Assert.Null(Writer);
            }
        }

        #endregion
    }
}
