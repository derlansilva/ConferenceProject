using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.models
{
    internal class ManifestSelection
    {
        public bool IsSelected { get; set; } // ✅ Checkbox de seleção
        public long Id { get; set; }         // 🆔 ID interno do banco
        public string ManifestNumber { get; set; } // 🔢 Número do Manifesto
        public string CreatedAt { get; set; }
    }
}
