using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backaramis.Binder
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            //Primero obtenemos el separador de miles para procesar la información aqui no nos interesa
            //el separador decimal porque a este es ya core y eso lo maneja perfecto el framework
            string? separadormiles = valueProviderResult.Culture.NumberFormat.CurrencyGroupSeparator;
             
            //obtenmos el valor del modelo 
            string value = valueProviderResult.FirstValue!;

            //si no hay valor finaliza si hacer nada
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // Removemos comas o puntos (separador de miles) nada mas
            // los decimales no importa si es separado por coma o punto 
            // dependiendo la region ya que eso lo manipula bien core
            value = value.Replace(separadormiles, ",").Trim();
            decimal myValue = Convert.ToDecimal(value);

            //retornamos el valor que si se manipula perfectamente en el controller
            //mapping o clase
            bindingContext.Result = ModelBindingResult.Success(myValue);
            return Task.CompletedTask;
        }
    }
}