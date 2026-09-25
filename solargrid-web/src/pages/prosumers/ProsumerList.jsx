import React, { useEffect, useState } from 'react';
import { prosumersApi } from '../../api/prosumersApi';
import { authApi } from '../../api/authApi';
import Button from '../../components/ui/Button';
import Input from '../../components/ui/Input';
import Modal from '../../components/ui/Modal';
import LoadingSpinner from '../../components/ui/LoadingSpinner';
import ErrorBanner from '../../components/ui/ErrorBanner';
import EmptyState from '../../components/ui/EmptyState';
import toast from 'react-hot-toast';
import { Plus, Edit2 } from 'lucide-react';

const ProsumerList = () => {
  const [prosumers, setProsumers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [saving, setSaving] = useState(false);

  const [newProsumer, setNewProsumer] = useState({ nic: '', fullName: '', phone: '', address: '', password: '' });
  const [editProsumer, setEditProsumer] = useState(null);

  useEffect(() => {
    fetchProsumers();
  }, []);

  const fetchProsumers = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await prosumersApi.getAll();
      setProsumers(data);
    } catch (err) {
      setError('Failed to load prosumers.');
    } finally {
      setLoading(false);
    }
  };

  const handleToggleStatus = async (prosumer) => {
    try {
      if (prosumer.isActive) {
        if (!window.confirm(`Are you sure you want to deactivate ${prosumer.fullName}?`)) return;
        await prosumersApi.deactivate(prosumer.nic);
        toast.success('Prosumer deactivated');
      } else {
        if (!window.confirm(`Are you sure you want to reactivate ${prosumer.fullName}?`)) return;
        await prosumersApi.reactivate(prosumer.nic);
        toast.success('Prosumer reactivated');
      }
      fetchProsumers();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update status');
    }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await authApi.register(newProsumer);
      toast.success('Prosumer created successfully');
      setIsCreateOpen(false);
      setNewProsumer({ nic: '', fullName: '', phone: '', address: '', password: '' });
      fetchProsumers();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to create prosumer');
    } finally {
      setSaving(false);
    }
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      await prosumersApi.update(editProsumer.nic, {
        fullName: editProsumer.fullName,
        phone: editProsumer.phone,
        address: editProsumer.address
      });
      toast.success('Prosumer updated successfully');
      setIsEditOpen(false);
      fetchProsumers();
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to update prosumer');
    } finally {
      setSaving(false);
    }
  };

  const openEdit = (prosumer) => {
    setEditProsumer(prosumer);
    setIsEditOpen(true);
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold text-gray-800">Prosumer Management</h2>
        <Button onClick={() => setIsCreateOpen(true)} className="flex items-center space-x-2">
          <Plus size={18} />
          <span>Add Prosumer</span>
        </Button>
      </div>

      <ErrorBanner message={error} />

      {loading ? (
        <div className="py-12"><LoadingSpinner /></div>
      ) : prosumers.length === 0 ? (
        <EmptyState title="No Prosumers Found" description="There are no prosumers registered in the system." />
      ) : (
        <div className="bg-white shadow overflow-hidden sm:rounded-lg border border-gray-200">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">NIC</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Full Name</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Phone</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {prosumers.map((prosumer) => (
                <tr key={prosumer.nic} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{prosumer.nic}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{prosumer.fullName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{prosumer.phone}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm">
                    {prosumer.isActive ? (
                      <span className="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                        Active
                      </span>
                    ) : (
                      <span className="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full bg-red-100 text-red-800">
                        Inactive
                      </span>
                    )}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
                    <Button variant="outline" className="text-xs py-1 px-2" onClick={() => openEdit(prosumer)}>
                      <Edit2 size={14} className="inline mr-1"/> Edit
                    </Button>
                    <Button 
                      variant={prosumer.isActive ? 'danger' : 'secondary'} 
                      onClick={() => handleToggleStatus(prosumer)}
                      className="text-xs py-1 px-2"
                    >
                      {prosumer.isActive ? 'Deactivate' : 'Reactivate'}
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Create Prosumer Modal */}
      <Modal isOpen={isCreateOpen} onClose={() => setIsCreateOpen(false)} title="Add New Prosumer">
        <form onSubmit={handleCreate} className="space-y-4">
          <Input label="NIC" value={newProsumer.nic} onChange={e => setNewProsumer({...newProsumer, nic: e.target.value})} required />
          <Input label="Full Name" value={newProsumer.fullName} onChange={e => setNewProsumer({...newProsumer, fullName: e.target.value})} required />
          <Input label="Phone" value={newProsumer.phone} onChange={e => setNewProsumer({...newProsumer, phone: e.target.value})} required />
          <Input label="Address" value={newProsumer.address} onChange={e => setNewProsumer({...newProsumer, address: e.target.value})} required />
          <Input label="Password" type="password" value={newProsumer.password} onChange={e => setNewProsumer({...newProsumer, password: e.target.value})} required />
          <div className="pt-4 flex justify-end space-x-3">
            <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>Cancel</Button>
            <Button type="submit" disabled={saving}>{saving ? 'Saving...' : 'Create Prosumer'}</Button>
          </div>
        </form>
      </Modal>

      {/* Edit Prosumer Modal */}
      {editProsumer && (
        <Modal isOpen={isEditOpen} onClose={() => setIsEditOpen(false)} title={`Edit Prosumer: ${editProsumer.nic}`}>
          <form onSubmit={handleUpdate} className="space-y-4">
            <Input label="Full Name" value={editProsumer.fullName} onChange={e => setEditProsumer({...editProsumer, fullName: e.target.value})} required />
            <Input label="Phone" value={editProsumer.phone} onChange={e => setEditProsumer({...editProsumer, phone: e.target.value})} required />
            <Input label="Address" value={editProsumer.address} onChange={e => setEditProsumer({...editProsumer, address: e.target.value})} required />
            <div className="pt-4 flex justify-end space-x-3">
              <Button type="button" variant="outline" onClick={() => setIsEditOpen(false)}>Cancel</Button>
              <Button type="submit" disabled={saving}>{saving ? 'Updating...' : 'Update Prosumer'}</Button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
};

export default ProsumerList;
