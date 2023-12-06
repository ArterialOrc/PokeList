$(async function () {
    async function getYandexData() {
        let response = await fetch("/yandex-data", {
            method: "GET",
            headers: { "Accept": "application/json", }
        })
        if (response.ok) {
            let data = await response.json()
            return data
        }
    }

    async function yandexInit() {
        let data = await getYandexData()

        const authSuggest = await window.YaAuthSuggest.init(
            {
                client_id: data['clientId'],
                response_type: data['responseType'],
                redirect_uri: data['redirectUri']
            },
            data['tokenPageOrigin'],
            {
                view: 'button',
                parentId: 'yandex',
                buttonView: 'main',
                buttonTheme: 'light',
                buttonSize: 'm',
                buttonBorderRadius: 0
            }
        );

        const { handler } = await authSuggest;
        const token = await handler();
        return token['access_token']
    }



    async function yandexEmail(token) {
        console.log('Сообщение с токеном', token);
        let response = await fetch("https://login.yandex.ru/info", {
            method: "GET",
            headers: { "Authorization": `OAuth ${token}`, }
        })
        if (response.ok) {
            let data = await response.json()
            await fetch('/yandex-login', {
                method: "POST",
                headers: { "Accept": "application/json", "Content-Type": "application/json" },
                body: JSON.stringify({
                    'email': data.default_email
                })
            });
        } else {
            console.log('Net otveta')
        }
    }

    $('#login').on('click', async function (event) {
        event.preventDefault()
        let email = $('#email').val()
        let password = $('#password').val()

        let response = await fetch("/login", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                'email': email,
                'password': password
            })
        })
        if (response.ok) {
            $('#input_email').val(email)
            $('.code__modal').modal('show')
        } else {
            let data = await response.json()
            alert(data['message'])
        }
    })

    $('#password-btn').on('click', async function (event) {
        console.log('PASSWORD')
        let response = await fetch("/pass-change/", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                'email': $('#email').val(),
            })
        })
        if (response.ok)
            console.log('PASS CHANGED')
        else
            console.log("ERROR")
    })

    let token = await yandexInit()
    let email = await yandexEmail(token)
});