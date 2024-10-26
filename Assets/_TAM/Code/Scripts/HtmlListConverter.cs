using UnityEngine;
using TMPro;

public class HtmlListConverter : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    void Start()
    {
        // Teks HTML yang ingin dikonversi
        string htmlText = "<ul><li>Kebijakan privasi & consent [Izin] wajib ditayangkan pada aplikasi, agar customer mengetahui dan menyetujui tujuan dan aktivitas dari pengumpulan & penggunaan data pada aplikasi tsb.</li><li>Karyawan dianjurkan untuk berkonsultasi dengan Corporate Legal & Compliance apabila kurang memahami mengenai Personal Data Protection.</li><li>Kewajiban pengumpulan Data pribadi [Pengumpulan & Pemrosesan Data Pribadi]</li></ul><ol><li>Memiliki persetujuan untuk pengumpulan & penggunaan data (Persetujuan)</li><li>Memberitahukan Pemilik Data Pribadi terlebih dahulu mengenai tujuan pengumpulan (Sepengetahuan)</li><li>Menggunakan & mengungkapkan bahwa data yang dikumpulkan hanya untuk tujuan awal [aktivitas pemrosesan] (Mengungkapkan)</li><li>Menghormati hak individu untuk menarik/mengubah persetujuan mereka (Menghormati hak)</li></ol>";

        // Konversi HTML ke format TextMeshPro
        string convertedText = ConvertHtmlToTextMeshPro(htmlText);

        // Set teks yang telah dikonversi ke TextMeshPro
        textMeshPro.text = convertedText;
    }

    private string ConvertHtmlToTextMeshPro(string htmlText)
    {
        // Hapus <p> tags
        htmlText = htmlText.Replace("<p>", "").Replace("</p>", "\n");

        // Konversi <b> menjadi TextMeshPro bold tag
        htmlText = htmlText.Replace("<b>", "<b>").Replace("</b>", "</b>");

        // Konversi <br> menjadi new line
        htmlText = htmlText.Replace("<br>", "\n");

        // Konversi <ul> menjadi daftar tidak berurutan dengan bullet points
        htmlText = ConvertUnorderedList(htmlText);

        // Konversi <ol> menjadi daftar berurutan dengan angka
        htmlText = ConvertOrderedList(htmlText);

        return htmlText;
    }

    private string ConvertOrderedList(string htmlText)
    {
        int itemIndex = 1;

        // Ganti <li> di dalam <ol> dengan angka dan tambahkan satu newline setelahnya
        while (htmlText.Contains("<ol>") || htmlText.Contains("</ol>"))
        {
            htmlText = htmlText.Replace("<ol>", "").Replace("</ol>", "");
        }

        while (htmlText.Contains("<li>"))
        {
            htmlText = htmlText.ReplaceFirst("<li>", itemIndex + ". ");
            htmlText = htmlText.ReplaceFirst("</li>", "\n");
            itemIndex++;
        }

        return htmlText;
    }

    private string ConvertUnorderedList(string htmlText)
    {
        // Ganti <li> di dalam <ul> dengan bullet point dan tambahkan satu newline setelahnya
        while (htmlText.Contains("<ul>") || htmlText.Contains("</ul>"))
        {
            htmlText = htmlText.Replace("<ul>", "").Replace("</ul>", "");
        }

        while (htmlText.Contains("<li>"))
        {
            htmlText = htmlText.ReplaceFirst("<li>", "<b>•</b> ");
            htmlText = htmlText.ReplaceFirst("</li>", "\n");
        }

        return htmlText;
    }

    // Fungsi untuk memberikan indentasi pada baris kedua dan seterusnya
    private string IndentSecondLine(string htmlText)
    {
        string[] lines = htmlText.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("1.") || lines[i].StartsWith("2.") || lines[i].StartsWith("<b>•</b>"))
            {
                continue;
            }
            lines[i] = "    " + lines[i]; // Tambahkan spasi pada baris selain yang dimulai dengan nomor atau bullet point
        }
        return string.Join("\n", lines);
    }
}

// Extension method untuk mengganti hanya kemunculan pertama dari sebuah string
public static class StringExtensions
{
    public static string ReplaceFirst(this string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
        {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
}
