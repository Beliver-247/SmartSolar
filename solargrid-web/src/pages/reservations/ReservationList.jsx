import React, { useEffect, useState } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { reservationsApi } from '../../api/reservationsApi';
import { stationsApi } from '../../api/stationsApi';
import Button from '../../components/ui/Button';
import Input from '../../components/ui/Input';
import Modal from '../../components/ui/Modal';
import LoadingSpinner from '../../components/ui/LoadingSpinner';
import ErrorBanner from '../../components/ui/ErrorBanner';
import EmptyState from '../../components/ui/EmptyState';
import toast from 'react-hot-toast';
import { Plus } from 'lucide-react';

const ReservationList = () => {
  const [reservations, setReservations] = useState([]);
  const [stations, setStations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Filters
  const [filterNic, setFilterNic] = useState('');
  const [filterStatus, setFilterStatus] = useState('');

  // Modals
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [newRes, setNewRes] = useState({ nic: '', stationId: '', bookingDate: '', slotId: '' });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);
      const [resData, statData] = await Promise.all([
        reservationsApi.getAll(),
        stationsApi.getAll()
      ]);
      
      setReservations(resData);
      setStations(statData);
    } catch (err) {
      setError('Failed to load reservations.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await reservationsApi.create({
        nic: newRes.nic,
        stationId: newRes.stationId,
        bookingDate: newRes.bookingDate,
        timeSlot: newRes.slotId // we stored the timeSlot string in slotId state for convenience
      });

      toast.success('Reservation created successfully');
      setIsCreateOpen(false);
      setNewRes({ nic: '', stationId: '', bookingDate: '', slotId: '' });
      fetchData();
    } catch (err) {
      toast.error(err.response?.data?.message || err.message || 'Failed to create reservation');
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = async (id) => {
    if (!window.confirm('Cancel this reservation?')) return;
    try {
      await reservationsApi.cancel(id);
      toast.success('Reservation cancelled');
      fetchData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to cancel (Check 12-hour rule)');
    }
  };

  const handleApprove = async (id) => {
    try {
      await reservationsApi.approve(id);
      toast.success('Reservation approved');
      fetchData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to approve');
    }
  };

  const handleComplete = async (id) => {
    if (!window.confirm('Mark this reservation as completed?')) return;
    try {
      await reservationsApi.complete(id);
      toast.success('Reservation completed');
      fetchData();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to modify reservation');
    }
  };

  // Filter Logic
  const filteredReservations = reservations.filter(r => {
    if (filterNic && !r.nic.toLowerCase().includes(filterNic.toLowerCase())) return false;
    if (filterStatus && r.status !== filterStatus) return false;
    return true;
  });

  const { user } = useAuth();
  const canBookOnBehalf = user?.role === 'Backoffice' || user?.role === 'GridOperator';

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold text-gray-800">Reservations Overview</h2>
        {canBookOnBehalf && (
          <Button onClick={() => setIsCreateOpen(true)} className="flex items-center space-x-2">
            <Plus size={18} />
            <span>Book on Behalf</span>
          </Button>
        )}
      </div>

      <div className="bg-white p-4 rounded-lg shadow border border-gray-200 flex space-x-4 items-end">
        <Input 
          label="Filter by NIC" 
          value={filterNic} 
          onChange={(e) => setFilterNic(e.target.value)} 
          placeholder="e.g. 123456789V"
        />
        <div className="flex flex-col">
          <label className="mb-1 text-sm font-medium text-gray-700">Filter by Status</label>
          <select 
            className="px-3 py-2 border border-gray-300 rounded-md shadow-sm bg-white text-sm"
            value={filterStatus}
            onChange={(e) => setFilterStatus(e.target.value)}
          >
            <option value="">All Statuses</option>
            <option value="Pending">Pending</option>
            <option value="Approved">Approved</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      </div>

      <ErrorBanner message={error} />

      {loading ? (
        <div className="py-12"><LoadingSpinner /></div>
      ) : filteredReservations.length === 0 ? (
        <EmptyState title="No Reservations Found" description="Try adjusting your filters or create a new booking." />
      ) : (
        <div className="bg-white shadow overflow-x-auto sm:rounded-lg border border-gray-200">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Reservation ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Prosumer NIC</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Station</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Slot</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredReservations.map((res) => (
                <tr key={res.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-mono text-gray-900 font-semibold">
                     {res.id.substring(18).toUpperCase()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{res.nic}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {stations.find(s => s.id === res.stationId)?.stationName || 'Unknown'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {res.bookingDate}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {res.timeSlot}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full 
                      ${res.status === 'Approved' ? 'bg-blue-100 text-blue-800' : 
                        res.status === 'Completed' ? 'bg-green-100 text-green-800' : 
                        res.status === 'Pending' ? 'bg-yellow-100 text-yellow-800' : 
                        'bg-gray-100 text-gray-800'}`}>
                      {res.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
                    {res.status === 'Pending' && (
                      <Button variant="outline" className="text-xs py-1 px-2" onClick={() => handleApprove(res.id)}>Approve</Button>
                    )}
                    {res.status === 'Approved' && (
                      <Button variant="secondary" className="text-xs py-1 px-2" onClick={() => handleComplete(res.id)}>Mark Complete</Button>
                    )}
                    {(res.status === 'Pending' || res.status === 'Approved') && (
                      <Button variant="danger" className="text-xs py-1 px-2" onClick={() => handleCancel(res.id)}>Cancel</Button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Book on Behalf">
        <form onSubmit={handleCreate} className="space-y-4">
          <Input 
            label="Prosumer NIC" 
            value={newRes.nic} 
            onChange={e => setNewRes({...newRes, nic: e.target.value})} 
            required 
            placeholder="E.g. 123456789V"
          />
          <div className="flex flex-col">
            <label className="mb-1 text-sm font-medium text-gray-700">Station</label>
            <select 
              className="px-3 py-2 border border-gray-300 rounded-md shadow-sm bg-white"
              value={newRes.stationId}
              onChange={e => setNewRes({...newRes, stationId: e.target.value})}
              required
            >
              <option value="">Select a Station</option>
              {stations.map(s => <option key={s.id} value={s.id}>{s.stationName}</option>)}
            </select>
          </div>
          <Input 
            label="Booking Date" 
            type="date"
            value={newRes.bookingDate} 
            onChange={e => setNewRes({...newRes, bookingDate: e.target.value})} 
            required 
          />
          <div className="flex flex-col">
            <label className="mb-1 text-sm font-medium text-gray-700">Time Slot</label>
            <select 
              className="px-3 py-2 border border-gray-300 rounded-md shadow-sm bg-white"
              value={newRes.slotId}
              onChange={e => setNewRes({...newRes, slotId: e.target.value})}
              required
              disabled={!newRes.stationId}
            >
              <option value="">{newRes.stationId ? 'Select Time Slot' : 'Select a Station First'}</option>
              {newRes.stationId && (stations.find(s => s.id === newRes.stationId)?.schedule || []).map((slot, index) => (
                <option key={index} value={slot}>{slot}</option>
              ))}
            </select>
          </div>
          <div className="pt-4 flex justify-end space-x-3">
            <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>Cancel</Button>
            <Button type="submit" disabled={saving}>{saving ? 'Booking...' : 'Confirm Booking'}</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default ReservationList;
