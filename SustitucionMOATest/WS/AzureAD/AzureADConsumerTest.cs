using Moq;
using NUnit.Framework;
using SustitucionMOAWS.AzureAD;
using SustitucionMOAWS.AzureAD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.WS.AzureAD
{
    [TestFixture()]
    public class AzureADConsumerTest
    {
        private Mock<IUsersGraphAPIClient> mIUsersGraphAPIClient;

        [SetUp]
        public void Setup()
        {
            mIUsersGraphAPIClient = new Mock<IUsersGraphAPIClient>();
        }

        [Test]
        public void BorrarUsuarioSegunMail_Ok()
        {
            var mailUsuario = "gthreep@mi.com";
            var idUsuarioAzure = "ffew24-dfdsgd-354tdk";
            var obtenerUsuarioResp = new ObtenerUsuarioResponse
            {
                Value = new List<UsuarioResponse>
                {
                    new UsuarioResponse { Id = idUsuarioAzure }
                }
            };

            mIUsersGraphAPIClient
                .Setup(x => x.ObtenerUsuarioPorDisplayName(mailUsuario))
                .Returns(obtenerUsuarioResp);

            mIUsersGraphAPIClient
                .Setup(x => x.BorrarUsuario(idUsuarioAzure));

            var target = new AzureADConsumer(mIUsersGraphAPIClient.Object);
            target.BorrarUsuarioSegunMail(mailUsuario);

            mIUsersGraphAPIClient.Verify(x => x.ObtenerUsuarioPorDisplayName(mailUsuario), Times.Once);
            mIUsersGraphAPIClient.Verify(x => x.BorrarUsuario(idUsuarioAzure), Times.Once);
        }
    }
}
