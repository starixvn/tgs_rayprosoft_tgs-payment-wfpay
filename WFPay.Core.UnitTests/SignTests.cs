using Microsoft.Extensions.Logging.Abstractions;
using Tgs.Payment.WFPay;

namespace WFPay.Core.UnitTests;

public class SignTests
{
    [Fact]
    public void 餘額查詢_CreateSign()
    {
        var dataString = """
            {"account_name":"amuse888","timestamp":"2024-09-25T09:28:43+08:00"}
            """;

        var privateKey = """
            -----BEGIN RSA PRIVATE KEY-----
            MIIEogIBAAKCAQEAonHdMURgQR5oKoFdv0FHPQ8bML+Ou4VpCKXRhx9qF6zw/xAD
            zzcolO9RoVqfVYnRtBJnLHdICrwR9wLbnYL5JpivXW/cUzY7w0OtZ1FU3WP5rzTt
            GR6idOfUlsWO98v+cAI6v+up5zNzJnfoqVZEftcYynQSt+s6q1K5immZOhPjvRDu
            JyFcaijgs+1M7Lub1Loyb+fHhK4ANqigjJ6WIk6/xSl81vOTDj/WolxhvFd0Y8Y8
            q8F5Ug6wHyIag6PD3NF/hLbo8HedivPAT0IW3/K0YtHg1uPBaT83b/MJgi9L6CCe
            EDh+FtnNaigCAdq3InPyFcI+bRtBtkul5uJbAwIDAQABAoIBABBaD972uA22BtxI
            e2xWINqnm6ATmEWOD12ce3PzmJVbhkLWlNuV/NLJKFlqquFMZzyOIa6bLzhNaC2K
            Lxsy5OuasQRhzzbdHbTxKhJdYIKMnsShOkCuEcJc18x5hIOzGCCkSM+q5EZuqmez
            6EqrcWenNUqj1oKG/jfisuSY7qCm2NAp+H0f2kQ0jULW7/ltPAvePNIjQ2cu3Yhc
            CFG2Jjxw9sLY5pdCcEVVJDAjNP+VPZsjIgpgrKYiN8HQOycfVI768f6EHIRrDYVG
            IOiB/h92YBA7yp1eA+FDGd1q7QTZJiuWEO8KUK23m5kSIPJzXPUZqOAcknnNVgs9
            zj39AUECgYEA00ACfGUp+dPCo3CzpFuAYYKXMAWxqH4oEEIxIHyvfKRxCIJ3eKtq
            ey3ONkijetoYn9isVAJqm8HETvzBGUSjo168tnpTj3agXSTHTejW1JbHd7B7CdCa
            NnAqL/XpOxrYrBGgDHsf2u7wWeES98DQmA/kRSADwxdqn2mJgI5jDKMCgYEAxNsr
            DrjpUoUPc7r4qpwgqp66Uv/Xm9hzXEqN50t7dg8HnjMR40G7K60qb9UvpDlaw0yO
            jSNky66GdGFwN8r991i7EWUcfUXbrYHu5O3klE2SyeLj+0ND3h9YWhLS4SfOpixo
            kRLk4bWKe2L8W+rNOD1DHQAWeEsadm1cYQlc/iECgYBMOEYtV9rjVvJdQiau4Izb
            oeCm5QcKwin2Y8uR6yruD+7woRI2KbMLqqVClMbXZqWH1wemcBDDDhCkZzHMyiU4
            ps/0DwLjOmv173UxuRb6MUTa/IQIMInTpniyQUnbl34h52IV3r7JoXh7FWhgl+Mr
            WmCESF7THznLNqr0BWvbfQKBgE4Y5SwiD+68+zReqOiGqC8IacMj3NXnOV0T9tgc
            Cg78R3lrqV0PZsIdIQjc+tmcJ3XzmVpFAGgOBSmGvo5j6f2HJi/wfSz12gEr2yrX
            dief3uxZJzk3pmUo6lRRw3ev4zvpwlJLvKIO87Gfw55zuJbnNwj6T+/JW+Y/3LZi
            kJIBAoGAI/GjrKmex4w8aHNT+UtRfCCoh1chKjdBFuuz3QNSLH+j/xIx6XzgMh50
            HwUENRf6FwbHAzATo4ht3FyzEIwSbchTpl5YKXQzMSvHU9OS0axFJhmZ0Z11GV9S
            8J1lA6/OgITirkJrSwdBcbYQmkmF3I8CdZpAzVC1tj76dziUIvI=
            -----END RSA PRIVATE KEY-----
            """;

        var sut = new WFPaySigner(NullLogger<WFPaySigner>.Instance);
        var actual = sut.CreateSign(dataString, privateKey);

        var expected = "CTTe7x7c2tJLXooI0lmBw9H2lKXywxTYhnuvxSo/OJjoG7ya41o8saAe9eUV5k3aJ0O4ZK7ECnDMhNAPm+aPLJjtuUg49nLYIW1Di/+7K3Xme2DW+5hVVumkoFG0JkoXfaI8ggfEPbYe4T+1GidBs+wPK7QsrInGAaQDiq0NJqLThcJAiZzMO3GVYMBjyEEpRC1LZbJlNdxL30Qx575dJ30wCygcY+rjHn124BVjtO//SB+0XUJKt07LL+d+7BffqxDOzzK+XiLHbFEBeifryP21cEGG6ktqLdKWZSvGbXegI8UpEah++1S3RKbZdIitA1fEJ7x6JASuOF3ZjCdUVg==";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void 代收_CreateSign()
    {
        var dataString = """
            {"account_name":"amuse888","merchant_order_id":"ee051ad1-05cb-4198-9aa1-e44d490a449d","total_amount":1000,"timestamp":"2024-09-25T06:32:00+00:00","notify_url":"https:\/\/jrfzmv4b-7185.asse.devtunnels.ms\/deposit\/callback","subject":"\u50a8\u503c","payment_method":"alipay"}
            """;

        var privateKey = """
            -----BEGIN RSA PRIVATE KEY-----
            MIIEogIBAAKCAQEAonHdMURgQR5oKoFdv0FHPQ8bML+Ou4VpCKXRhx9qF6zw/xAD
            zzcolO9RoVqfVYnRtBJnLHdICrwR9wLbnYL5JpivXW/cUzY7w0OtZ1FU3WP5rzTt
            GR6idOfUlsWO98v+cAI6v+up5zNzJnfoqVZEftcYynQSt+s6q1K5immZOhPjvRDu
            JyFcaijgs+1M7Lub1Loyb+fHhK4ANqigjJ6WIk6/xSl81vOTDj/WolxhvFd0Y8Y8
            q8F5Ug6wHyIag6PD3NF/hLbo8HedivPAT0IW3/K0YtHg1uPBaT83b/MJgi9L6CCe
            EDh+FtnNaigCAdq3InPyFcI+bRtBtkul5uJbAwIDAQABAoIBABBaD972uA22BtxI
            e2xWINqnm6ATmEWOD12ce3PzmJVbhkLWlNuV/NLJKFlqquFMZzyOIa6bLzhNaC2K
            Lxsy5OuasQRhzzbdHbTxKhJdYIKMnsShOkCuEcJc18x5hIOzGCCkSM+q5EZuqmez
            6EqrcWenNUqj1oKG/jfisuSY7qCm2NAp+H0f2kQ0jULW7/ltPAvePNIjQ2cu3Yhc
            CFG2Jjxw9sLY5pdCcEVVJDAjNP+VPZsjIgpgrKYiN8HQOycfVI768f6EHIRrDYVG
            IOiB/h92YBA7yp1eA+FDGd1q7QTZJiuWEO8KUK23m5kSIPJzXPUZqOAcknnNVgs9
            zj39AUECgYEA00ACfGUp+dPCo3CzpFuAYYKXMAWxqH4oEEIxIHyvfKRxCIJ3eKtq
            ey3ONkijetoYn9isVAJqm8HETvzBGUSjo168tnpTj3agXSTHTejW1JbHd7B7CdCa
            NnAqL/XpOxrYrBGgDHsf2u7wWeES98DQmA/kRSADwxdqn2mJgI5jDKMCgYEAxNsr
            DrjpUoUPc7r4qpwgqp66Uv/Xm9hzXEqN50t7dg8HnjMR40G7K60qb9UvpDlaw0yO
            jSNky66GdGFwN8r991i7EWUcfUXbrYHu5O3klE2SyeLj+0ND3h9YWhLS4SfOpixo
            kRLk4bWKe2L8W+rNOD1DHQAWeEsadm1cYQlc/iECgYBMOEYtV9rjVvJdQiau4Izb
            oeCm5QcKwin2Y8uR6yruD+7woRI2KbMLqqVClMbXZqWH1wemcBDDDhCkZzHMyiU4
            ps/0DwLjOmv173UxuRb6MUTa/IQIMInTpniyQUnbl34h52IV3r7JoXh7FWhgl+Mr
            WmCESF7THznLNqr0BWvbfQKBgE4Y5SwiD+68+zReqOiGqC8IacMj3NXnOV0T9tgc
            Cg78R3lrqV0PZsIdIQjc+tmcJ3XzmVpFAGgOBSmGvo5j6f2HJi/wfSz12gEr2yrX
            dief3uxZJzk3pmUo6lRRw3ev4zvpwlJLvKIO87Gfw55zuJbnNwj6T+/JW+Y/3LZi
            kJIBAoGAI/GjrKmex4w8aHNT+UtRfCCoh1chKjdBFuuz3QNSLH+j/xIx6XzgMh50
            HwUENRf6FwbHAzATo4ht3FyzEIwSbchTpl5YKXQzMSvHU9OS0axFJhmZ0Z11GV9S
            8J1lA6/OgITirkJrSwdBcbYQmkmF3I8CdZpAzVC1tj76dziUIvI=
            -----END RSA PRIVATE KEY-----
            """;

        var sut = new WFPaySigner(NullLogger<WFPaySigner>.Instance);
        var actual = sut.CreateSign(dataString, privateKey);

        var expected = "DmCm+ve9WM/T+lYgh4qnboekkJoGo3mEpXFVjYs5Kd21Hlt/be4bBTj4TUUtin/4xaHidPBk4+RXIwo0q15uZyXf9dymJvP0Gh1U6VVSfmASmOqKVJyOD181abiwIocxI21or0OxqDV91ALEttN7DD5VzOYw8WV2P5CJcKpEjRd3U/LXugGGBLY8BdE7+D81W4f0nk9BU25D/UFCSaVMoiWSh9S8dLyx5llftjosieuZvQnBCHnbscXjuDCbteB4SqNqDPaj/eY2iHWwk7yoiI/wf15/CB3APWZKjR9PBFghHXG0EF5ZRFPslkwA/u+bUflc10KlBqdN2fwaDxQXkA==";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void 代收回調_VerifySign()
    {
        var data = """
            {"order":{"id":"c89127ee-5e01-4c92-942b-ed8a32e9b3b3","subject":"储值","total_amount":"1000.0","notify_url":"https://jrfzmv4b-7185.asse.devtunnels.ms/deposit/callback","return_url":null,"merchant_order_id":"b002ca16-3d4d-485f-ab00-20829fc05718","status":"completed","payment_url":"https://gnngbwtf.com/orders/c89127ee-5e01-4c92-942b-ed8a32e9b3b3?token=eyJhbGciOiJSUzI1NiJ9.eyJvcmRlcl9pZCI6ImM4OTEyN2VlLTVlMDEtNGM5Mi05NDJiLWVkOGEzMmU5YjNiMyJ9.YHg6ly8GG0PJDsUvxWS-4uFeBY_6tFCYjyBw4gNJPZagLd6XiO8oX5lsV7F1mTwuWZAkgBr0mWCt4mvr8SENYNlP8-fA9m6ivpALWwRBpjg_bojLaDaGTkgNjkq7wSrBy41qXvCpXaiH93stYJwwI4Rfhgs8O-01a36eaYuJQ5UR_FgdTKdP4hk9LA4fmje0wptkJLRymNoSH9ooDhSsjrUYo_5wZcDC7NilOQ2LPOzfL0rMQZVDK5q2s06BEY-v5TcgLl4UGrzgOOSYjdDaf90if9lirOuTSTpaW0w1LPOnFrbQUlvZshK_RS1hapFUdOXo5OZoDAo_gRK5WR5g2Q","merchant_fee":"43.0","confirmation_code":"34939","remark":null,"bank_account":null,"qrcode_image_url":null,"qrcode_url":null,"payment_image_url":null,"supplement_orders":[],"supplement_orders_status":"none","payment_info":{"account_name":"18137030255","surname":"孟","name":"孟"},"currency":"CNY","completed_at":"2024-09-26T11:08:06+08:00","created_at":"2024-09-26T10:10:45+08:00"},"notify_type":"trade_completed"}
            """;

        var sign = "w3AB8LTIJcmP9rKcDJmGDrr5SFIBzDkSMjeg33B1jOMraDwn1Rq+e+LC4QDWbORYYBYO55WAB5Uwv8lcyDR2NPWEPsiS+qL/BPEIxH31+/KTP+q0xWpWgcDjPyX/VOYPsO6oFzEbL/R0wlD+rTqT2edKiY2W9knMF1tvOZkAvQFPw0xqEUkzjcX/iV/UjtVSO5P37QJtKp4ppSQjbZtUQtpXKeyG8SdhnqLgS+CkVDPtYCdcvAzxhaQcXyeibzRv0pZP/Iahv3DzcEFbTraHaiI/mo7JLV5sPBTDuU+ydLs1JIdSIav5bSJEacdyFbngtbaOx0y7lCF2XDwF9x6Opw==";

        var publicKey = """
            -----BEGIN PUBLIC KEY-----
            MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6SBH3gH+A275RHoqMlc/
            C932XKDPbha+gkJhdW7jypy5pzzE7y3y+1xD5m8tA+vpPXMT2fl62R4H+wr3aiau
            QupXq24yZAq5b3D5x8lnRSoHcSymDWeSr0e/anZcI6IlqbXfo05Ta4j4+F7ThAR0
            TMK3mrvTQPvAR9OhiHOkY/JKSDAo2/yERZKtPVWfH3U5XPiVcXoI3psGUpoGy3DB
            cUGvuH1ArvFHrl9JllfLAcQQ5b/1m+VD5PT5PCcSA0exg9cxsrS+oih4u18OmHi/
            aVamgr9B44a9NKXj8bJuS9RhPCcV35xw3h9X+TKkBpVgD9yhql+qkeyrQx79WBFj
            hQIDAQAB
            -----END PUBLIC KEY-----
            """;

        var sut = new WFPaySigner(NullLogger<WFPaySigner>.Instance);
        var actual = sut.VerifySign(data, sign, publicKey);

        Assert.True(actual);
    }    
}