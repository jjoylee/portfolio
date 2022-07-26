﻿namespace Lib.Payment.Alipay.Service
{
    public class RefundService : IRefundService
    {
        [Autowire]
        public IOrderDao OrderDao { get; set; }
        [Autowire]
        public IPgResultDao PgResultDao { get; set; }
        [Autowire]
        public IAlipayGatewayService AlipayGatewayService { get; set; }

        public void Request(int orderId, int siteId, int refundPrice)
        {
            try
            {
                var outRefundNo = "outRefundNo";
                var request = GetRequest(outRefundNo, orderId, siteId, refundPrice);
                var response = AlipayGatewayService.Get(siteId).Execute(request);
                if (response.Code != "10000") throw new Exception($"退款失败，请重新尝试");      
            }
            catch (Exception ex)
            {
                throw new BizException("AlipayRefund", ex.Message);
            }
        }

        private RefundRequest GetRequest(string outRefundNo, int orderId, int siteId, int refundPrice)
        {
            var item = PgResultDao.FindItem(orderId, siteId, "PAID");
            if (item == null) throw new Exception("您还没有已付款记录");
            var request = new RefundRequest();
            request.AddGatewayData(new RefundModel()
            {
                OutTradeNo = item.PaymentId,
                RefundAmount = refundPrice,
                OutRefundNo = outRefundNo
            });
            return request;
        }
    }
}
