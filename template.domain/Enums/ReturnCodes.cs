using System.ComponentModel;

namespace template.domain.Enums
{
    public enum ReturnCodes
    {
        [Description("Success")]
        Ok = 0,

        [Description("Nenhum valor encontrado.")]
        NotFound = -1,

        [Description("Valor inválido.")]
        InvalidValue = -2,

        [Description("Credenciais inválidas.")]
        InvalidUserCredentials = -3,

        [Description("Sua sessão expirou. Por favor faça o login novamente.")]
        ExpiredToken = -4,

        [Description("Delete operation unsuccessful")]
        DeleteRequestDenied = -5,

        [Description("Create operation unsuccessful")]
        CreateRequestDenied = -6,

        [Description("Update operation unsuccessful")]
        UpdateRequestDenied = -7,

        [Description("Internal error")]
        InternalError = -9,

        [Description("Senha inválida.")]
        InvalidPassword = -10,

        [Description("Usuário inválido.")]
        InvalidUser = -11,

        [Description("Senhas diferentes.")]
        DifferentPasswords = -12,

        [Description("Tipo inválido.")]
        InvalidType = -13,

        [Description("Usuário já existe.")]
        UserExist = -14,

        [Description("Usuário ou senha inválido.")]
        InvalidUserPassword = -15,

        [Description("Request Vazio.")]
        EmptyRequest = -16,

        [Description("Exceção do Try-Catch")]
        ExceptionEx = -17,

        [Description("Acesso negado.")]
        AccessDenied = -18,

        [Description("Aguardando Liberação da administração")]
        UserTempAdd = -19,

        [Description("Sessão do sistema expirou! Por favor entre novamente!")]
        ExpiredTokenSystem = -20,

        [Description("E-mail não enviado!")]
        NotSend = -21,

        [Description("Valores não Aceitos!")]
        NotAcceptable = -22,

        [Description("Senha expirada. Por favor redefina sua senha!")]
        ResetPassword = -23

    }
}
