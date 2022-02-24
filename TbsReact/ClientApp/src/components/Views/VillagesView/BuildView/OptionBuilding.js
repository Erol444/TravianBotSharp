import { Button, Modal, Box } from "@mui/material";
import React, { useState } from "react";

import OptionBuildingModal from "./Modal/OptionBuildingModal";
import modalStyle from "../../../../styles/modal";

const OptionBuilding = () => {
	const [open, setOpen] = useState(false);
	const handleOpen = () => setOpen(true);
	const handleClose = () => setOpen(false);

	return (
		<>
			<Button
				type="submit"
				variant="contained"
				value="submit"
				color="warning"
				onClick={handleOpen}
			>
				Option
			</Button>
			<Modal open={open} onClose={handleClose}>
				<Box sx={modalStyle}>
					<OptionBuildingModal />
				</Box>
			</Modal>
		</>
	);
};

export default OptionBuilding;
