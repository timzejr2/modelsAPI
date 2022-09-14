using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using apimodels.Context;
using apimodels.models;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace apimodels.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MaqueteController : ControllerBase
    {
        private readonly MaqueteContext _context;
        private readonly string _connectionString;
        private readonly string _tableMaquetes;
        private readonly string _maquetes;
        private readonly string _fotosMaquetes;
        private string ErrorMessage { get; set; }
        public decimal filesize { get; set; }

        public MaqueteController(MaqueteContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetValue<string>("BlobConnectionString");
            _tableMaquetes = configuration.GetValue<string>("AzureTableName");
            _maquetes = configuration.GetValue<string>("BlobContainerMaquete");
            _fotosMaquetes = configuration.GetValue<string>("BlobContainerMaqueteFoto");
        }

        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(_connectionString);
            var TableClient = serviceClient.GetTableClient(_tableMaquetes);

            TableClient.CreateIfNotExists();
            return TableClient;
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Maquete>>> Listar()
        {
            var listaMaquetes = await _context.Maquetes.ToListAsync();
            return Ok(listaMaquetes);
        }

        [HttpGet("Obter/{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var maquete = await _context.Maquetes.FindAsync(id);

            if (maquete == null) return NotFound();

            return Ok(maquete);
        }

        [HttpGet("Obter/{nome}")]
        public async Task<IActionResult> ObterPorNome(string nome)
        {
            var maquete = await _context.Maquetes.FindAsync(nome);

            if (maquete == null) return NotFound();

            return Ok(maquete);
        }

        [HttpPost("NovaMaquete")]
        public async Task<IActionResult> Cadastrar(Maquete maquete)
        {
            await _context.AddAsync(maquete);
            await _context.SaveChangesAsync();

            var tableClient = GetTableClient();
            var maquetesLog = new MaquetesLog(maquete, TipoAcao.Inclusao, maquete.IdUsuario.ToString(), Guid.NewGuid().ToString());

            maquetesLog.RowKey = Guid.NewGuid().ToString();
            maquetesLog.PartitionKey = maquetesLog.RowKey;

            await tableClient.UpsertEntityAsync(maquetesLog);

            return CreatedAtAction(nameof(ObterPorId), new { id = maquete.Id }, maquete);
        }

        [HttpPost("NovaMaquete/photo")]
        public async Task<IActionResult> UploadPhotoMaquete(IFormFile arquivo)
        {
            filesize = 3072;

            if (arquivo == null) return BadRequest();

            var supportedTypes = new[] { "jpg", "jpeg", "img", "bitmap", "png" };
            var fileExt = System.IO.Path.GetExtension(arquivo.FileName).Substring(1);
            Console.WriteLine(fileExt);
            if (!supportedTypes.Contains(fileExt))
            {
                ErrorMessage = "File Extension Is InValid - Only Upload jpg/jpeg/png/bitmap File";
                return BadRequest(ErrorMessage);
            }
            else if (arquivo.Length > (filesize * 1024))
            {
                ErrorMessage = "File size Should Be UpTo " + filesize + "KB";
                return BadRequest(ErrorMessage);
            }
            else
            {
                BlobContainerClient container = new(_connectionString, _fotosMaquetes);

                BlobClient blob = container.GetBlobClient($"{Guid.NewGuid()}-{arquivo.FileName}");
                System.Console.WriteLine(arquivo.FileName);
                using (var ms = new MemoryStream())
                {
                    await arquivo.CopyToAsync(ms);
                    ms.Position = 0;
                    await blob.UploadAsync(ms);
                }

                return Ok(blob.Uri.AbsoluteUri);
            }
        }

        [HttpPost("NovaMaquete/maquete")]
        public async Task<IActionResult> UploadMaquete(IFormFileCollection arquivos)
        {
            if (arquivos == null) return BadRequest();

            string folderName = $"{Guid.NewGuid()}";

            string[] extensao = new string[4];

            BlobContainerClient container = new(_connectionString, _maquetes);

            foreach (var arquivo in arquivos)
            {
                BlobClient blob;

                extensao = arquivo.FileName.Split('.');

                if(extensao[1]=="framework" && extensao[2]=="js" && extensao[3]=="unityweb")
                {
                    blob = container.GetBlobClient($"{folderName}/{folderName}.{extensao[1]}.{extensao[2]}.{extensao[3]}");
                }
                else
                {
                    blob = container.GetBlobClient($"{folderName}/{folderName}.{extensao[1]}.{extensao[2]}");
                }

                using (var ms = new MemoryStream())
                {
                    await arquivo.CopyToAsync(ms);
                    ms.Position = 0;
                    await blob.UploadAsync(ms);
                }
            }

            return Ok(folderName);
        }

        [HttpPut("Alterar/{id}")]
        public async Task<IActionResult> Atualizar(int id, Maquete maquete)
        {
            var maqueteBanco = await _context.Maquetes.FindAsync(id);

            if (maqueteBanco == null) return NotFound();

            maqueteBanco.NomeMaquete = maquete.NomeMaquete;
            maqueteBanco.DescricaoMaquete = maquete.DescricaoMaquete;
            maqueteBanco.MaqueteAtiva = maquete.MaqueteAtiva;

            _context.Maquetes.Update(maqueteBanco);
            await _context.SaveChangesAsync();

            var tableClient = GetTableClient();
            var maquetesLog = new MaquetesLog(maquete, TipoAcao.Inclusao, maquete.IdUsuario.ToString(), Guid.NewGuid().ToString());

            await tableClient.UpsertEntityAsync(maquetesLog);

            return Ok();
        }

        [HttpDelete("ExcluirMaquete/{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var maqueteBanco = await _context.Maquetes.FindAsync(id);

            if (maqueteBanco == null) return NotFound();

            _context.Maquetes.Remove(maqueteBanco);
            await _context.SaveChangesAsync();

            var tableClient = GetTableClient();
            var maquetesLog = new MaquetesLog(maqueteBanco, TipoAcao.Inclusao, maqueteBanco.IdUsuario.ToString(), Guid.NewGuid().ToString());

            await tableClient.UpsertEntityAsync(maquetesLog);

            return NoContent();
        }

        [HttpDelete("ExcluirPhotoMaquete/{Photo}")]
        public IActionResult DeletarFoto( string Photo)
        {
            BlobContainerClient container = new(_connectionString, _fotosMaquetes);
            BlobClient blob = container.GetBlobClient(Photo);

            blob.DeleteIfExists();
            return NoContent();
        }

        [HttpDelete("ExcluirMaquete/{Maquete}")]
        public IActionResult DeletarMaquete( string Maquete)
        {
            string arquivo1 = Maquete + "/" + Maquete + ".data.unityweb";
            string arquivo2 = Maquete + "/" + Maquete + ".framework.js.unityweb";
            string arquivo3 = Maquete + "/" + Maquete + ".loader.js";
            string arquivo4 = Maquete + "/" + Maquete + ".wasm.unityweb";

            BlobContainerClient container = new(_connectionString, _maquetes);
            BlobClient blob1 = container.GetBlobClient(arquivo1);
            BlobClient blob2 = container.GetBlobClient(arquivo2);
            BlobClient blob3 = container.GetBlobClient(arquivo3);
            BlobClient blob4 = container.GetBlobClient(arquivo4);

            blob1.DeleteIfExists();
            blob2.DeleteIfExists();
            blob3.DeleteIfExists();
            blob4.DeleteIfExists();
            return NoContent();
        }
    }
}