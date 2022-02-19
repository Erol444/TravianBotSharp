import { Button, Modal } from "@mui/material";
import React, { useState } from "react";

import ButtonBuildingModal from "./Modal/ButtonBuildingModal";

const ButtonBuilding = () => {
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
				Add building
			</Button>
			<Modal open={open} onClose={handleClose}>
				<ButtonBuildingModal />
			</Modal>
		</>
	);
};

export default ButtonBuilding;
