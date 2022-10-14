using System.Security.Claims;

namespace backaramis.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// 

        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (!user.Identity!.IsAuthenticated)
            {
                return null!;
            }

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value + ' ' + currentUser.FindFirst(ClaimTypes.GivenName)!.Value;
        }

        public static string GetUserPerfil(this ClaimsPrincipal user)
        {
            if (!user.Identity!.IsAuthenticated)
            {
                return null!;
            }

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.Role)!.Value;
        }
        //aramis

        public static string NumeroLetras(decimal numero)
        {
            string ImpLetra;
            string lcRetorno = string.Empty;
            string lcCadena;
            int lnEntero = (int)numero;
            int lnTerna = 1;
            int lnUnidades;
            int lnDecenas;
            int lnCentenas;
            int lnFraccion = (int)((numero - lnEntero) * 100);

            while (lnEntero > 0)
            {
                lcCadena = string.Empty;
                lnUnidades = lnEntero % 10;
                lnEntero /= 10;
                lnDecenas = lnEntero % 10;
                lnEntero /= 10;
                lnCentenas = lnEntero % 10;
                lnEntero /= 10;
                lcCadena = lnUnidades.Equals(1) && lnTerna.Equals(1) ? "Uno " + lcCadena :
                           lnUnidades.Equals(1) && !lnTerna.Equals(1) ? "Un " + lcCadena :
                           lnUnidades.Equals(2) ? "Dos " + lcCadena :
                           lnUnidades.Equals(3) ? "Tres " + lcCadena :
                           lnUnidades.Equals(4) ? "Cuatro " + lcCadena :
                           lnUnidades.Equals(5) ? "Cinco " + lcCadena :
                           lnUnidades.Equals(6) ? "Seis " + lcCadena :
                           lnUnidades.Equals(7) ? "Siete " + lcCadena :
                           lnUnidades.Equals(8) ? "Ocho " + lcCadena :
                           lnUnidades.Equals(9) ? "Nueve " + lcCadena :
                           lcCadena;

                lcCadena = lnDecenas.Equals(1) ? (
                           lnUnidades.Equals(0) ? "Diez " :
                           lnUnidades.Equals(1) ? "Once " :
                           lnUnidades.Equals(2) ? "Doce " :
                           lnUnidades.Equals(3) ? "Trece " :
                           lnUnidades.Equals(4) ? "Catorce " :
                           lnUnidades.Equals(5) ? "Quince " :
                           lnUnidades.Equals(6) ? "Dieciseis " :
                           lnUnidades.Equals(7) ? "Diecisiete " :
                           lnUnidades.Equals(8) ? "Dieciocho " :
                           lnUnidades.Equals(9) ? "Diecinueve " :
                           lcCadena) :

                           lnDecenas.Equals(2) ? (
                           lnUnidades.Equals(0) ? "Veinte " + lcCadena : "Veinti" + lcCadena) :
                           lnDecenas.Equals(3) ? (
                           lnUnidades.Equals(0) ? "Treinta " + lcCadena : "Treinta y " + lcCadena) :
                           lnDecenas.Equals(4) ? (
                           lnUnidades.Equals(0) ? "Cuarenta " + lcCadena : "Cuarenta y " + lcCadena) :
                           lnDecenas.Equals(5) ? (
                           lnUnidades.Equals(0) ? "Cincuenta " + lcCadena : "Cincuenta y " + lcCadena) :
                           lnDecenas.Equals(6) ? (
                           lnUnidades.Equals(0) ? "Sesenta " + lcCadena : "Sesenta y " + lcCadena) :
                           lnDecenas.Equals(7) ? (
                           lnUnidades.Equals(0) ? "Setenta " + lcCadena : "Setenta y " + lcCadena) :
                           lnDecenas.Equals(8) ? (
                           lnUnidades.Equals(0) ? "Ochenta " + lcCadena : "Ochenta y " + lcCadena) :
                           lnDecenas.Equals(9) ? (
                           lnUnidades.Equals(0) ? "Noventa " + lcCadena : "Noventa y " + lcCadena) :
                           lcCadena;

                lcCadena = lnCentenas.Equals(1) && lnTerna.Equals(3) ? "Cien " + lcCadena :
                           lnCentenas.Equals(1) && lnUnidades.Equals(0) && lnDecenas.Equals(0) ? "Cien " + lcCadena :
                           lnCentenas.Equals(1) && !lnTerna.Equals(3) ? "Ciento " + lcCadena :
                           lnCentenas.Equals(2) ? "Doscientos " + lcCadena :
                           lnCentenas.Equals(3) ? "Trescientos " + lcCadena :
                           lnCentenas.Equals(4) ? "Cuatrocientos " + lcCadena :
                           lnCentenas.Equals(5) ? "Quinientos " + lcCadena :
                           lnCentenas.Equals(6) ? "Seiscientos " + lcCadena :
                           lnCentenas.Equals(7) ? "Sietecientos " + lcCadena :
                           lnCentenas.Equals(8) ? "Ochocientos " + lcCadena :
                           lnCentenas.Equals(9) ? "Novecientos " + lcCadena :
                           lcCadena;
                lcCadena = lnTerna.Equals(1) ? lcCadena :
                          lnTerna.Equals(2) && !(lnUnidades + lnDecenas + lnCentenas).Equals(0) ? lcCadena + "Mil " :
                          lnTerna.Equals(3) && !(lnUnidades + lnDecenas + lnCentenas).Equals(0) && (lnUnidades.Equals(1) && lnDecenas.Equals(0) && lnCentenas.Equals(0)) ? lcCadena + "Millón " :
                          lnTerna.Equals(3) && !(lnUnidades + lnDecenas + lnCentenas).Equals(0) && !(lnUnidades.Equals(1) && lnDecenas.Equals(0) && lnCentenas.Equals(0)) ? lcCadena + "Millones " :
                          lnTerna.Equals(4) && !(lnUnidades + lnDecenas + lnCentenas).Equals(0) ? lcCadena + "Mil Millones " :
                          String.Empty;

                lcRetorno = lcCadena + lcRetorno;
                lnTerna++;
            }

            if (lnTerna.Equals(1))
            {
                lcRetorno = "Cero";
            }

            string _fraccion = "00" + lnFraccion.ToString().TrimStart();
            ImpLetra = string.Concat(lcRetorno.TrimEnd(), " con ", _fraccion.AsSpan((_fraccion.Length) - 2, 2), "/100");
            if (ImpLetra.StartsWith("Un") && ImpLetra.Contains("Millones"))
            {
                ImpLetra = ImpLetra.Replace("Millones", "Millón");
            }

            return ImpLetra;
        }
    }
}