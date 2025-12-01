using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// Stores workflow vouchers in memory and returns them on demand.
    /// </summary>
    public sealed class WorkflowVoucherLoader : IWorkflowVoucherLoader
    {
        private readonly IDictionary<int, IDictionary<int, IWorkflowVoucher>> _vouchersByKind;

        public WorkflowVoucherLoader()
            : this(new Dictionary<int, IDictionary<int, IWorkflowVoucher>>())
        {
        }

        public WorkflowVoucherLoader(IDictionary<int, IDictionary<int, IWorkflowVoucher>> vouchersByKind)
        {
            _vouchersByKind = vouchersByKind ?? new Dictionary<int, IDictionary<int, IWorkflowVoucher>>();
        }

        public void RegisterVoucher(IWorkflowVoucher voucher)
        {
            if (voucher == null)
            {
                throw new ArgumentNullException(nameof(voucher));
            }

            IDictionary<int, IWorkflowVoucher> bucket;
            if (!_vouchersByKind.TryGetValue(voucher.Kind, out bucket))
            {
                bucket = new Dictionary<int, IWorkflowVoucher>();
                _vouchersByKind[voucher.Kind] = bucket;
            }

            bucket[voucher.Id] = voucher;
        }

        public IWorkflowVoucher GetWorkflowVoucher(int voucherId, int voucherKind)
        {
            IDictionary<int, IWorkflowVoucher> bucket;
            if (!_vouchersByKind.TryGetValue(voucherKind, out bucket))
            {
                throw new InvalidOperationException(string.Format("No vouchers are registered for kind {0}.", voucherKind));
            }

            IWorkflowVoucher voucher;
            if (!bucket.TryGetValue(voucherId, out voucher))
            {
                throw new InvalidOperationException(string.Format("Voucher {0} was not found for kind {1}.", voucherId, voucherKind));
            }

            return voucher;
        }
    }
}
