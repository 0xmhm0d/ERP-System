import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
  Typography,
  Box,
  InputAdornment,
  IconButton,
  Tooltip,
} from '@mui/material';
import { Add as AddIcon, Remove as RemoveIcon } from '@mui/icons-material';

const AddItemDialog = ({ open, onClose, onAdd, item }) => {
  const [formData, setFormData] = useState({
    itemName: '',
    quantity: '',
    unitPrice: '',
    totalPrice: '',
  });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    if (item) {
      setFormData({
        itemName: item.itemName || '',
        quantity: item.quantity || '',
        unitPrice: item.unitPrice || '',
        totalPrice: item.totalPrice || '',
      });
    } else {
      setFormData({
        itemName: '',
        quantity: '',
        unitPrice: '',
        totalPrice: '',
      });
    }
  }, [item]);

  const validateForm = () => {
    const newErrors = {};
    if (!formData.itemName.trim()) {
      newErrors.itemName = 'Item name is required';
    }
    if (!formData.quantity) {
      newErrors.quantity = 'Quantity is required';
    } else if (isNaN(formData.quantity) || formData.quantity <= 0) {
      newErrors.quantity = 'Quantity must be greater than 0';
    }
    if (!formData.unitPrice) {
      newErrors.unitPrice = 'Unit price is required';
    } else if (isNaN(formData.unitPrice) || formData.unitPrice <= 0) {
      newErrors.unitPrice = 'Unit price must be greater than 0';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => {
      const newData = { ...prev, [name]: value };
      if (name === 'quantity' || name === 'unitPrice') {
        const quantity = name === 'quantity' ? value : prev.quantity;
        const unitPrice = name === 'unitPrice' ? value : prev.unitPrice;
        if (quantity && unitPrice) {
          newData.totalPrice = (parseFloat(quantity) * parseFloat(unitPrice)).toFixed(2);
        }
      }
      return newData;
    });
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleQuantityChange = (increment) => {
    const currentQty = parseFloat(formData.quantity) || 0;
    const newQty = Math.max(1, currentQty + increment);
    setFormData(prev => ({
      ...prev,
      quantity: newQty.toString(),
      totalPrice: (newQty * parseFloat(prev.unitPrice || 0)).toFixed(2)
    }));
  };

  const handleSubmit = () => {
    if (validateForm()) {
      onAdd(formData);
      onClose();
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        <Typography variant="h6" component="div" sx={{ fontWeight: 'bold' }}>
          {item ? 'Edit Item' : 'Add New Item'}
        </Typography>
      </DialogTitle>
      <DialogContent>
        <Box sx={{ mt: 2 }}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Item Name"
                name="itemName"
                value={formData.itemName}
                onChange={handleChange}
                error={!!errors.itemName}
                helperText={errors.itemName}
                required
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Quantity"
                name="quantity"
                type="number"
                value={formData.quantity}
                onChange={handleChange}
                error={!!errors.quantity}
                helperText={errors.quantity}
                required
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Tooltip title="Decrease quantity">
                        <IconButton
                          size="small"
                          onClick={() => handleQuantityChange(-1)}
                          edge="start"
                        >
                          <RemoveIcon />
                        </IconButton>
                      </Tooltip>
                    </InputAdornment>
                  ),
                  endAdornment: (
                    <InputAdornment position="end">
                      <Tooltip title="Increase quantity">
                        <IconButton
                          size="small"
                          onClick={() => handleQuantityChange(1)}
                          edge="end"
                        >
                          <AddIcon />
                        </IconButton>
                      </Tooltip>
                    </InputAdornment>
                  ),
                }}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Unit Price"
                name="unitPrice"
                type="number"
                value={formData.unitPrice}
                onChange={handleChange}
                error={!!errors.unitPrice}
                helperText={errors.unitPrice}
                required
                InputProps={{
                  startAdornment: <InputAdornment position="start">$</InputAdornment>,
                }}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Total Price"
                name="totalPrice"
                value={formData.totalPrice}
                InputProps={{
                  readOnly: true,
                  startAdornment: <InputAdornment position="start">$</InputAdornment>,
                }}
              />
            </Grid>
          </Grid>
        </Box>
      </DialogContent>
      <DialogActions sx={{ p: 2 }}>
        <Button onClick={onClose} color="inherit">
          Cancel
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          color="primary"
          sx={{ minWidth: 100 }}
        >
          {item ? 'Update' : 'Add'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddItemDialog; 