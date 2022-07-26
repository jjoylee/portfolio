﻿namespace Lib.Payment.Alipay.Service
{
    public class OrderQueryService : IOrderQueryService
    {
        [Autowire]
        public IAlipayGatewayService AlipayGatewayService { get; set; }

        [Autowire]
        public IPayStateSyncService PayStateSyncService { get; set; }

        public QueryResponse Request(string outTradeNo, int orderId, int siteId)
        {
            try
            {
                var request = GetRequest(outTradeNo);
                var response = AlipayGatewayService.Get(siteId).Execute(request);
                if (response.SubCode != "ACQ.TRADE_NOT_EXIST" && response.Code != "10000") throw new Exception("失败，请重新尝试");
                if (response.TradeStatus == "TRADE_SUCCESS") PayStateSyncService.Sync(outTradeNo, response.ToJson(), orderId, (int)response.TotalAmount);
                return response;
            }
            catch (Exception e)
            {
                throw new BizException("OrderQueryService", e.Message);
            }
        }

        private QueryRequest GetRequest(string outTradeNo)
        {
            var request = new QueryRequest();
            request.AddGatewayData(new QueryModel()
            {
                OutTradeNo = outTradeNo
            });
            return request;
        }
    }
}
