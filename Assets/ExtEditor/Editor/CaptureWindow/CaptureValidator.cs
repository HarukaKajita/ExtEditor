using System;
using System.IO;
using UnityEngine;

namespace ExtEditor.Editor.CaptureWindow
{
    /// <summary>
    /// キャプチャ設定のバリデーション機能を提供するクラス
    /// </summary>
    public static class CaptureValidator
    {
        /// <summary>
        /// パスの有効性をチェック
        /// </summary>
        public static ValidationResult ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new ValidationResult(false, "パスが空です", "Path is empty");
            }
            
            // 不正文字チェック
            char[] invalidChars = Path.GetInvalidPathChars();
            if (path.IndexOfAny(invalidChars) >= 0)
            {
                return new ValidationResult(false, "パスに不正な文字が含まれています", "Path contains invalid characters");
            }
            
            // セキュリティチェック
            if (path.Contains("..") || path.Contains("~"))
            {
                return new ValidationResult(false, "危険なパスパターンが検出されました", "Dangerous path pattern detected");
            }
            
            try
            {
                Path.GetFullPath(path);
                return new ValidationResult(true, "", "");
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, $"パスエラー: {ex.Message}", $"Path Error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// ファイル名の有効性をチェック
        /// </summary>
        public static ValidationResult ValidateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new ValidationResult(false, "ファイル名が空です", "Filename is empty");
            }
            
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidChars) >= 0)
            {
                return new ValidationResult(false, "ファイル名に不正な文字が含まれています", "Filename contains invalid characters");
            }
            
            return new ValidationResult(true, "", "");
        }
        
        /// <summary>
        /// ファイル名テンプレートの有効性をチェック
        /// </summary>
        public static ValidationResult ValidateTemplate(string template)
        {
            if (string.IsNullOrEmpty(template))
            {
                return new ValidationResult(false, "テンプレートが空です", "Template is empty");
            }
            
            // 基本的なパターンが含まれているかチェック
            bool hasValidPattern = template.Contains("<Date>") || 
                                 template.Contains("<Time>") || 
                                 template.Contains("<Take>") ||
                                 template.Contains("<Name>");
            
            if (!hasValidPattern)
            {
                return new ValidationResult(false, "有効なパターンが含まれていません", "No valid patterns found");
            }
            
            return new ValidationResult(true, "", "");
        }
    }
    
    /// <summary>
    /// バリデーション結果を表すクラス
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string MessageJapanese { get; }
        public string MessageEnglish { get; }
        
        public ValidationResult(bool isValid, string messageJapanese, string messageEnglish)
        {
            IsValid = isValid;
            MessageJapanese = messageJapanese;
            MessageEnglish = messageEnglish;
        }
        
        public string GetMessage(CaptureWindow.Language language)
        {
            return language == CaptureWindow.Language.Japanese ? MessageJapanese : MessageEnglish;
        }
    }
}