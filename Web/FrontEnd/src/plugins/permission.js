const permissions = {
    //Usuários
    U_V: "U_V", //Visualizar
    U_C: "U_C", //Criar
    U_U: "U_U", //Alterar
    U_D: "U_D", //Deletar
    U_UPWD: "U_UPWD", //Altera a senha

    //Grupo de autorizações
    AG_V: "AG_V", //Visualizar
    AG_C: "AG_C", //Criar
    AG_U: "AG_U", //Alterar
    AG_D: "AG_D", //Deletar

};
export default {
    install: (app, options) => {
        app.config.globalProperties.$permissions = permissions;
    },
    permissions
}